using HOTEL_PEA2.Data;
using HOTEL_PEA2.Models;
using HOTEL_PEA2.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HOTEL_PEA2.Controllers
{
    public class ReservaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET: /Reserva
        // Lista de reservas con filtro opcional por nombre de cliente.
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

            var listaReservas = await query.ToListAsync();
            return View(listaReservas);
        }

        // ============================================================
        // GET: /Reserva/NuevaReserva
        // Carga la vista con los datos necesarios para el formulario:
        //   - Reserva nueva o existente
        //   - Listas para los <select>
        //   - Datos extra para el JavaScript (auto-relleno y filtro)
        // ============================================================
        public async Task<IActionResult> NuevaReserva(int? id)
        {
            ReservaViewModel vm = new ReservaViewModel();

            // --------------------------------------------------------
            // 1) Cargar la reserva (nueva o existente)
            // --------------------------------------------------------
            if (id == null || id == 0)
            {
                vm.Reserva = new Reserva
                {
                    FechaEntrada = DateTime.Today,
                    FechaSalida = DateTime.Today.AddDays(1),
                    Estado = "Pendiente",
                    CantidadPersonas = 1
                };
            }
            else
            {
                vm.Reserva = await _context.Reserva.FindAsync(id);
                if (vm.Reserva == null) return NotFound();
            }

            // --------------------------------------------------------
            // 2) Listas simples para los <select>
            // --------------------------------------------------------
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

            // --------------------------------------------------------
            // 3) Datos extra para el JavaScript
            // --------------------------------------------------------

            // Clientes: con DNI, teléfono, email y nombre completo
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

            // Habitaciones: con precio, piso y tipo (Id + nombre)
            // ⬅️ CAMBIO: se agrega TipoId para poder filtrar por Id de tipo
            ViewBag.HabitacionesData = await _context.Habitacion
                .Include(h => h.TipoHabitacion)
                .Select(h => new
                {
                    Id = h.IdHabitacion,
                    Numero = h.Numero,
                    Precio = h.Precio,
                    Piso = h.Piso,
                    TipoId = h.IdTipoHabitacion,   // ⬅️ NUEVO
                    Tipo = h.TipoHabitacion != null ? h.TipoHabitacion.NombreTipo : ""
                })
                .ToListAsync();

            // ⬅️ NUEVO: Reservas activas (para que JS sepa qué habitaciones están ocupadas)
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
        // Maneja dos casos:
        //   A) Cliente existente → usa su IdCliente
        //   B) Cliente nuevo     → crea primero el Cliente y luego la Reserva
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarReserva(
            ReservaViewModel vm,
            string TipoCliente = "existente")   // ⬅️ NUEVO: viene del hidden
        {
            // Validación básica
            if (vm == null || vm.Reserva == null)
            {
                return BadRequest();
            }

            // --------------------------------------------------------
            // A) CLIENTE NUEVO: crear primero el Cliente
            // --------------------------------------------------------
            if (TipoCliente == "nuevo")
            {
                // Validar que tengamos los datos mínimos del cliente
                if (vm.Cliente == null ||
                    string.IsNullOrWhiteSpace(vm.Cliente.Dni) ||
                    string.IsNullOrWhiteSpace(vm.Cliente.Nombres))
                {
                    ModelState.AddModelError("", "Debe completar los datos del cliente nuevo.");
                    return View("NuevaReserva", vm);
                }

                // Verificar que el DNI no exista ya
                var existeDni = await _context.Cliente
                    .AnyAsync(c => c.Dni == vm.Cliente.Dni);

                if (existeDni)
                {
                    ModelState.AddModelError("Cliente.Dni",
                        "Ya existe un cliente con ese DNI.");
                    return View("NuevaReserva", vm);
                }

                // Crear el nuevo Cliente
                var nuevoCliente = new Cliente
                {
                    Dni = vm.Cliente.Dni,
                    Nombres = vm.Cliente.Nombres,   // viene del campo "Nombre Completo"
                    Apellidos = "",                  // se puede dividir si quieres
                    Telefono = vm.Cliente.Telefono,
                    Email = vm.Cliente.Email
                };

                _context.Cliente.Add(nuevoCliente);

                // Guardar para que se genere el IdCliente
                await _context.SaveChangesAsync();

                // Asignar el IdCliente recién creado a la reserva
                vm.Reserva.IdCliente = nuevoCliente.IdCliente;
            }
            // --------------------------------------------------------
            // B) CLIENTE EXISTENTE: usar el IdCliente seleccionado
            // --------------------------------------------------------
            else
            {
                if (vm.Reserva.IdCliente == 0)
                {
                    ModelState.AddModelError("Reserva.IdCliente",
                        "Debe seleccionar un cliente.");
                    return View("NuevaReserva", vm);
                }
            }

            // --------------------------------------------------------
            // C) Guardar la reserva (nueva o edición)
            // --------------------------------------------------------
            if (vm.Reserva.IdReserva == 0)
            {
                // Nueva reserva
                _context.Reserva.Add(vm.Reserva);
            }
            else
            {
                // Editar reserva existente
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
                {
                    reservaEnDb.TipoHabitacion = vm.Reserva.TipoHabitacion;
                }

                _context.Reserva.Update(reservaEnDb);
            }

            await _context.SaveChangesAsync();
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

            return RedirectToAction(nameof(Index));
        }
    }
}