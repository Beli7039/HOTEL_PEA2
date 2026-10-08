using HOTEL_PEA2.Data;
using HOTEL_PEA2.Models;
using HOTEL_PEA2.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
//using CrystalDecisions.CrystalReports.Engine;

namespace HOTEL_PEA2.Controllers
{
    public class ReservaController : Controller
    {
        private readonly ApplicationDbContext _context;

        // ============================================================
        // CONSTANTES DE NEGOCIO
        // Cargo por gestión administrativa cuando hay modificaciones.
        // ============================================================
        private const decimal PORCENTAJE_CARGO_ADMIN = 0.10m;   // 10%
        private const decimal CARGO_ADMIN_MINIMO = 20.00m;  // S/ 20.00 mínimo

        public ReservaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // Helper: calcular el cargo administrativo
        // ============================================================
        private decimal CalcularCargoAdministrativo(decimal montoBase)
        {
            var cargo = montoBase * PORCENTAJE_CARGO_ADMIN;
            return cargo < CARGO_ADMIN_MINIMO ? CARGO_ADMIN_MINIMO : cargo;
        }

        // ============================================================
        // GET: /Reserva
        // ============================================================
        public async Task<IActionResult> Index(string criterio)
        {
            var query = _context.Reserva
                .Include(r => r.Cliente)
                .Include(r => r.Habitacion)
                .AsQueryable();

            if (!string.IsNullOrEmpty(criterio))
            {
                query = query.Where(r => r.Cliente != null &&
                    (r.Cliente.Nombres.Contains(criterio) ||
                     r.Cliente.Apellidos.Contains(criterio)));
            }

            var listaReservas = await query
                .OrderByDescending(r => r.FechaEntrada)
                .ToListAsync();

            ViewBag.Criterio = criterio;
            return View(listaReservas);
        }

        // ============================================================
        // GET: /Reserva/NuevaReserva
        // ============================================================
        public async Task<IActionResult> NuevaReserva(int? id)
        {
            ReservaViewModel vm = new ReservaViewModel();

            if (id == null || id == 0)
            {
                vm.Reserva = new Reserva
                {
                    FechaEntrada = DateTime.Today,
                    FechaSalida = DateTime.Today.AddDays(1),
                    Estado = "Pendiente",
                    CantidadPersonas = 1,
                    CargoAdministrativo = 0
                };
            }
            else
            {
                vm.Reserva = await _context.Reserva.FindAsync(id);
                if (vm.Reserva == null) return NotFound();
            }

            vm.ListaClientes = await _context.Cliente
                .Select(x => new SelectListItem
                {
                    Text = x.Nombres + " " + x.Apellidos,
                    Value = x.IdCliente.ToString()
                }).ToListAsync();

            vm.ListaHabitaciones = await _context.Habitacion
                .Select(x => new SelectListItem
                {
                    Text = x.Numero,
                    Value = x.IdHabitacion.ToString()
                }).ToListAsync();

            vm.ListaTipoHabitacion = await _context.Tipo_Habitacion
                .Select(x => new SelectListItem
                {
                    Text = x.NombreTipo,
                    Value = x.IdTipoHabitacion.ToString()
                }).ToListAsync();

            ViewBag.ClientesData = await _context.Cliente
                .Select(c => new
                {
                    Id = c.IdCliente,
                    Nombre = c.Nombres + " " + c.Apellidos,
                    Dni = c.Dni,
                    Telefono = c.Telefono,
                    Email = c.Email
                })
                .ToListAsync();

            ViewBag.HabitacionesData = await _context.Habitacion
                .Include(h => h.TipoHabitacion)
                .Select(h => new
                {
                    Id = h.IdHabitacion,
                    Numero = h.Numero,
                    Precio = h.Precio,
                    Piso = h.Piso,
                    TipoId = h.IdTipoHabitacion,
                    Tipo = h.TipoHabitacion != null ? h.TipoHabitacion.NombreTipo : ""
                })
                .ToListAsync();

            ViewBag.ReservasExistentes = await _context.Reserva
                .Where(r => r.Estado != "Cancelada")
                .Select(r => new
                {
                    IdReserva = r.IdReserva,
                    HabitacionId = r.IdHabitacion,
                    FechaEntrada = r.FechaEntrada,
                    FechaSalida = r.FechaSalida
                })
                .ToListAsync();

            return View(vm);
        }

        // ============================================================
        // POST: /Reserva/GuardarReserva
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarReserva(
            ReservaViewModel vm,
            string TipoCliente = "existente")
        {
            if (vm == null || vm.Reserva == null)
                return BadRequest();

            // A) CLIENTE NUEVO
            if (TipoCliente == "nuevo")
            {
                if (vm.Cliente == null ||
                    string.IsNullOrWhiteSpace(vm.Cliente.Dni) ||
                    string.IsNullOrWhiteSpace(vm.Cliente.Nombres))
                {
                    ModelState.AddModelError("", "Debe completar los datos del cliente nuevo.");
                    return View("NuevaReserva", vm);
                }

                var existeDni = await _context.Cliente
                    .AnyAsync(c => c.Dni == vm.Cliente.Dni);

                if (existeDni)
                {
                    ModelState.AddModelError("Cliente.Dni", "Ya existe un cliente con ese DNI.");
                    return View("NuevaReserva", vm);
                }

                var nuevoCliente = new Cliente
                {
                    Dni = vm.Cliente.Dni,
                    Nombres = vm.Cliente.Nombres,
                    Apellidos = "",
                    Telefono = vm.Cliente.Telefono,
                    Email = vm.Cliente.Email
                };

                _context.Cliente.Add(nuevoCliente);
                await _context.SaveChangesAsync();

                vm.Reserva.IdCliente = nuevoCliente.IdCliente;
            }
            // B) CLIENTE EXISTENTE
            else
            {
                if (vm.Reserva.IdCliente == 0)
                {
                    ModelState.AddModelError("Reserva.IdCliente", "Debe seleccionar un cliente.");
                    return View("NuevaReserva", vm);
                }
            }

            // C) GUARDAR
            if (vm.Reserva.IdReserva == 0)
            {
                // Nueva reserva → sin cargo administrativo
                vm.Reserva.CargoAdministrativo = 0;
                _context.Reserva.Add(vm.Reserva);
            }
            else
            {
                var reservaEnDb = await _context.Reserva.FindAsync(vm.Reserva.IdReserva);
                if (reservaEnDb == null) return NotFound();

                reservaEnDb.FechaEntrada = vm.Reserva.FechaEntrada;
                reservaEnDb.FechaSalida = vm.Reserva.FechaSalida;
                reservaEnDb.CantidadPersonas = vm.Reserva.CantidadPersonas;
                reservaEnDb.CostoTotal = vm.Reserva.CostoTotal;
                reservaEnDb.Estado = vm.Reserva.Estado;
                reservaEnDb.Observaciones = vm.Reserva.Observaciones;
                reservaEnDb.IdCliente = vm.Reserva.IdCliente;
                reservaEnDb.IdHabitacion = vm.Reserva.IdHabitacion;
                reservaEnDb.IdRecepcionista = vm.Reserva.IdRecepcionista;

                if (!string.IsNullOrEmpty(vm.Reserva.TipoHabitacion))
                    reservaEnDb.TipoHabitacion = vm.Reserva.TipoHabitacion;

                _context.Reserva.Update(reservaEnDb);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // GET: /Reserva/Reprogramar/5
        // ============================================================
        public async Task<IActionResult> Reprogramar(int id)
        {
            var reserva = await _context.Reserva
                .Include(r => r.Cliente)
                .Include(r => r.Habitacion)
                .FirstOrDefaultAsync(r => r.IdReserva == id);

            if (reserva == null) return NotFound();

            if (reserva.Estado == "Cancelada")
            {
                TempData["Error"] = "No se puede reprogramar una reserva cancelada.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PorcentajeCargo = PORCENTAJE_CARGO_ADMIN;
            ViewBag.CargoMinimo = CARGO_ADMIN_MINIMO;

            return View(reserva);
        }

        // ============================================================
        // POST: /Reserva/Reprogramar
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reprogramar(int IdReserva, DateTime FechaEntrada,
                                                     DateTime FechaSalida, int CantidadPersonas)
        {
            var reserva = await _context.Reserva
                .Include(r => r.Habitacion)
                .FirstOrDefaultAsync(r => r.IdReserva == IdReserva);

            if (reserva == null) return NotFound();

            // Validar fechas
            if (FechaSalida <= FechaEntrada)
            {
                ModelState.AddModelError("", "La fecha de salida debe ser posterior a la de entrada.");
                ViewBag.PorcentajeCargo = PORCENTAJE_CARGO_ADMIN;
                ViewBag.CargoMinimo = CARGO_ADMIN_MINIMO;
                return View(reserva);
            }

            // Verificar disponibilidad
            var ocupada = await _context.Reserva.AnyAsync(r =>
                r.IdReserva != IdReserva &&
                r.IdHabitacion == reserva.IdHabitacion &&
                r.Estado != "Cancelada" &&
                r.FechaEntrada < FechaSalida &&
                r.FechaSalida > FechaEntrada);

            if (ocupada)
            {
                ModelState.AddModelError("",
                    "La habitación ya está reservada en ese rango de fechas.");
                ViewBag.PorcentajeCargo = PORCENTAJE_CARGO_ADMIN;
                ViewBag.CargoMinimo = CARGO_ADMIN_MINIMO;
                return View(reserva);
            }

            // Recalcular base y aplicar cargo administrativo
            var precio = reserva.Habitacion?.Precio ?? 0;
            var noches = (FechaSalida - FechaEntrada).Days;
            var nuevoBase = noches * precio;

            // El cargo se calcula sobre el nuevo total base
            var cargoNuevo = CalcularCargoAdministrativo(nuevoBase);

            // Acumular al cargo existente
            reserva.CargoAdministrativo += cargoNuevo;

            reserva.FechaEntrada = FechaEntrada;
            reserva.FechaSalida = FechaSalida;
            reserva.CantidadPersonas = CantidadPersonas;
            reserva.CostoTotal = nuevoBase;

            await _context.SaveChangesAsync();

            TempData["Exito"] = $"Reserva reprogramada. Se agregó un cargo administrativo de S/ {cargoNuevo:F2}.";
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // GET: /Reserva/ModificarHabitacion/5
        // ============================================================
        public async Task<IActionResult> ModificarHabitacion(int id)
        {
            var reserva = await _context.Reserva
                .Include(r => r.Cliente)
                .Include(r => r.Habitacion)
                    .ThenInclude(h => h.TipoHabitacion)
                .FirstOrDefaultAsync(r => r.IdReserva == id);

            if (reserva == null) return NotFound();

            if (reserva.Estado == "Cancelada")
            {
                TempData["Error"] = "No se puede modificar una reserva cancelada.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TiposHabitacion = await _context.Tipo_Habitacion
                .AsNoTracking()
                .ToListAsync();

            ViewBag.HabitacionesData = await _context.Habitacion
                .Include(h => h.TipoHabitacion)
                .AsNoTracking()
                .Select(h => new
                {
                    Id = h.IdHabitacion,
                    Numero = h.Numero,
                    Precio = h.Precio,
                    Piso = h.Piso,
                    TipoId = h.IdTipoHabitacion,
                    Tipo = h.TipoHabitacion != null ? h.TipoHabitacion.NombreTipo : ""
                })
                .ToListAsync();

            ViewBag.ReservasExistentes = await _context.Reserva
                .Where(r => r.Estado != "Cancelada")
                .AsNoTracking()
                .Select(r => new
                {
                    IdReserva = r.IdReserva,
                    HabitacionId = r.IdHabitacion,
                    FechaEntrada = r.FechaEntrada,
                    FechaSalida = r.FechaSalida
                })
                .ToListAsync();

            ViewBag.PorcentajeCargo = PORCENTAJE_CARGO_ADMIN;
            ViewBag.CargoMinimo = CARGO_ADMIN_MINIMO;

            return View(reserva);
        }

        // ============================================================
        // POST: /Reserva/ModificarHabitacion
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModificarHabitacion(int IdReserva, int IdHabitacion,
                                                              string TipoHabitacion)
        {
            var reserva = await _context.Reserva.FindAsync(IdReserva);
            if (reserva == null) return NotFound();

            if (IdHabitacion == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar una habitación.");
                return await RecargarVistaModificar(reserva);
            }

            var habitacion = await _context.Habitacion.FindAsync(IdHabitacion);
            if (habitacion == null)
            {
                ModelState.AddModelError("", "La habitación seleccionada no existe.");
                return await RecargarVistaModificar(reserva);
            }

            // Verificar disponibilidad
            var ocupada = await _context.Reserva.AnyAsync(r =>
                r.IdReserva != IdReserva &&
                r.IdHabitacion == IdHabitacion &&
                r.Estado != "Cancelada" &&
                r.FechaEntrada < reserva.FechaSalida &&
                r.FechaSalida > reserva.FechaEntrada);

            if (ocupada)
            {
                ModelState.AddModelError("",
                    "La habitación seleccionada ya está ocupada en las fechas de esta reserva.");
                return await RecargarVistaModificar(reserva);
            }

            // Recalcular base y aplicar cargo administrativo
            var noches = (reserva.FechaSalida - reserva.FechaEntrada).Days;
            var nuevoBase = noches * habitacion.Precio;

            var cargoNuevo = CalcularCargoAdministrativo(nuevoBase);

            reserva.CargoAdministrativo += cargoNuevo;

            reserva.IdHabitacion = IdHabitacion;
            reserva.TipoHabitacion = TipoHabitacion;
            reserva.CostoTotal = nuevoBase;

            await _context.SaveChangesAsync();

            TempData["Exito"] = $"Habitación modificada. Se agregó un cargo administrativo de S/ {cargoNuevo:F2}.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> RecargarVistaModificar(Reserva reserva)
        {
            ViewBag.TiposHabitacion = await _context.Tipo_Habitacion.AsNoTracking().ToListAsync();

            ViewBag.HabitacionesData = await _context.Habitacion
                .Include(h => h.TipoHabitacion)
                .AsNoTracking()
                .Select(h => new
                {
                    Id = h.IdHabitacion,
                    Numero = h.Numero,
                    Precio = h.Precio,
                    Piso = h.Piso,
                    TipoId = h.IdTipoHabitacion,
                    Tipo = h.TipoHabitacion != null ? h.TipoHabitacion.NombreTipo : ""
                })
                .ToListAsync();

            ViewBag.ReservasExistentes = await _context.Reserva
                .Where(r => r.Estado != "Cancelada")
                .AsNoTracking()
                .Select(r => new
                {
                    IdReserva = r.IdReserva,
                    HabitacionId = r.IdHabitacion,
                    FechaEntrada = r.FechaEntrada,
                    FechaSalida = r.FechaSalida
                })
                .ToListAsync();

            ViewBag.PorcentajeCargo = PORCENTAJE_CARGO_ADMIN;
            ViewBag.CargoMinimo = CARGO_ADMIN_MINIMO;

            return View("ModificarHabitacion", reserva);
        }

        // ============================================================
        // GET: /Reserva/Cancelar/5
        // ============================================================
        public async Task<IActionResult> Cancelar(int id)
        {
            var reserva = await _context.Reserva
                .Include(r => r.Cliente)
                .Include(r => r.Habitacion)
                .FirstOrDefaultAsync(r => r.IdReserva == id);

            if (reserva == null) return NotFound();

            if (reserva.Estado == "Cancelada")
            {
                TempData["Error"] = "Esta reserva ya está cancelada.";
                return RedirectToAction(nameof(Index));
            }

            return View(reserva);
        }

        // ============================================================
        // POST: /Reserva/Cancelar
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarConfirmado(int IdReserva, string Motivo)
        {
            var reserva = await _context.Reserva.FindAsync(IdReserva);
            if (reserva == null) return NotFound();

            reserva.Estado = "Cancelada";

            // Concatenar observaciones con el motivo y la política de no devolución
            var notaCancelacion = $"CANCELADA - SIN DERECHO A DEVOLUCIÓN";
            if (!string.IsNullOrWhiteSpace(Motivo))
                notaCancelacion += $". Motivo: {Motivo}";

            reserva.Observaciones = string.IsNullOrWhiteSpace(reserva.Observaciones)
                ? notaCancelacion
                : $"{reserva.Observaciones} | {notaCancelacion}";

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Reserva cancelada. No aplica devolución según la política del hotel.";
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // POST: /Reserva/Eliminar/5
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null) return NotFound();

            _context.Reserva.Remove(reserva);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Reserva eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ReporteReserva()
        {
            // 1. Consulta con todos tus campos (incluyendo Salida y Observaciones)
            var datosReporte = await _context.Reserva
                .Include(r => r.Cliente)
                .Select(r => new
                {
                    CodigoReserva = r.IdReserva,
                    DniCliente = r.Cliente.Dni,
                    NombreCliente = r.Cliente.Nombres,
                    HoraLlegada = r.FechaEntrada,
                    FechaSalida = r.FechaSalida,    
                    Observaciones = r.Observaciones,
                    CostoTotal = r.CostoTotal
                })
                .ToListAsync();

            var pdfBytes = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Opcional: Si tienes muchas columnas, puedes usar orientación Horizontal (Landscape) para que entre holgado
                    page.Size(QuestPDF.Helpers.PageSizes.A4.Landscape());
                    page.Margin(1, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);

                    // ==========================================
                    // TÍTULO: Centrado, Mayúsculas y Negro
                    // ==========================================
                    page.Header().AlignCenter().PaddingBottom(15).Text("REPORTE RESUMIDO DE RESERVAS")
                        .Bold().FontSize(16).FontColor(QuestPDF.Helpers.Colors.Black);

                    // ==========================================
                    // CONTENIDO: Tabla con todas las columnas
                    // ==========================================
                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40); // Código
                            columns.ConstantColumn(65); // DNI
                            columns.RelativeColumn(10);  // Cliente
                            columns.RelativeColumn(6);  // Llegada
                            columns.RelativeColumn(6);  // Salida
                            columns.RelativeColumn(25);// Observaciones
                            columns.ConstantColumn(65); // Costo Total
                        });

                        // Cabeceras de la tabla
                        table.Header(header =>
                        {
                            header.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(4).Text("Cód.").Bold();
                            header.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(4).Text("DNI").Bold();
                            header.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(4).Text("Cliente").Bold();
                            header.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(4).Text("Llegada").Bold();
                            header.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(4).Text("Salida").Bold();
                            header.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(4).Text("Observaciones").Bold();
                            header.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(4).Text("Total").Bold();
                        });

                        // Filas dinámicas de la base de datos
                        foreach (var item in datosReporte)
                        {
                            table.Cell().Padding(4).Text(item.CodigoReserva.ToString());
                            table.Cell().Padding(4).Text(item.DniCliente ?? "");
                            table.Cell().Padding(4).Text(item.NombreCliente ?? "");
                            table.Cell().Padding(4).Text(item.HoraLlegada.ToString("dd/MM/yyyy"));
                            table.Cell().Padding(4).Text(item.FechaSalida.ToString("dd/MM/yyyy"));
                            table.Cell().Padding(4).Text(item.Observaciones ?? "-");
                            table.Cell().Padding(4).Text($"S/ {item.CostoTotal:N2}");
                        }
                    });

                    // Pie de página
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", "Reporte_Reservas_Completo.pdf");
        }
    }
}