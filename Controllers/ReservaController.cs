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

        // GET: /Reserva
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

        // GET: /Reserva/NuevaReserva
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
                    CantidadPersonas = 1
                };
            }
            else
            {
                vm.Reserva = await _context.Reserva.FindAsync(id);
                if (vm.Reserva == null)
                {
                    return NotFound();
                }
            }

            // Listas simples para los dropdowns del ViewModel
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

            // === DATOS EXTRA PARA EL AUTO-RELLENO (data-*) ===

            // Clientes: DNI, Teléfono, Email
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

            // Habitaciones: Precio, Piso, Tipo
            ViewBag.HabitacionesData = await _context.Habitacion
                .Include(h => h.TipoHabitacion)
                .Select(h => new
                {
                    Id = h.IdHabitacion,
                    Numero = h.Numero,
                    Precio = h.Precio,
                    Piso = h.Piso,
                    TipoNombre = h.TipoHabitacion != null ? h.TipoHabitacion.NombreTipo : ""
                })
                .ToListAsync();

            return View(vm);
        }

        // POST: /Reserva/GuardarReserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarReserva(ReservaViewModel vm)
        {
            // Si el modelo de la reserva viene vacío, evitamos NullReference
            if (vm == null || vm.Reserva == null)
            {
                return BadRequest();
            }

            if (vm.Reserva.IdReserva == 0)
            {
                // Nueva reserva
                _context.Reserva.Add(vm.Reserva);
            }
            else
            {
                // Editar reserva existente
                var reservaEnDb = await _context.Reserva.FindAsync(vm.Reserva.IdReserva);
                if (reservaEnDb == null)
                {
                    return NotFound();
                }

                reservaEnDb.FechaEntrada = vm.Reserva.FechaEntrada;
                reservaEnDb.FechaSalida = vm.Reserva.FechaSalida;
                reservaEnDb.CantidadPersonas = vm.Reserva.CantidadPersonas;

                if (!string.IsNullOrEmpty(vm.Reserva.TipoHabitacion))
                {
                    reservaEnDb.TipoHabitacion = vm.Reserva.TipoHabitacion;
                }

                reservaEnDb.CostoTotal = vm.Reserva.CostoTotal;
                reservaEnDb.Estado = vm.Reserva.Estado;
                reservaEnDb.Observaciones = vm.Reserva.Observaciones;
                reservaEnDb.IdCliente = vm.Reserva.IdCliente;
                reservaEnDb.IdHabitacion = vm.Reserva.IdHabitacion;
                reservaEnDb.IdRecepcionista = vm.Reserva.IdRecepcionista;

                _context.Reserva.Update(reservaEnDb);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Reserva/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            _context.Reserva.Remove(reserva);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}