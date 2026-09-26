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
        public async Task<IActionResult> Index(string? criterio)
        {
            var query = _context.Reserva
                .Include(r => r.Cliente)
                .Include(r => r.Habitacion)
                .Include(r => r.Recepcionista)
                .AsQueryable();

            if (!string.IsNullOrEmpty(criterio))
            {
                query = query.Where(r =>
                    r.Cliente!.Nombres.Contains(criterio) ||
                    r.Cliente!.Apellidos.Contains(criterio));
            }

            var lista = await query.OrderByDescending(r => r.IdReserva).ToListAsync();
            ViewBag.Criterio = criterio;
            return View(lista);
        }

        // GET: /Reserva/NuevaReserva
        public IActionResult NuevaReserva()
        {
            var vm = new ReservaViewModel
            {
                vm.Reserva = new Reserva();
            }
            else
            {
                vm.Reserva = await _context.Reserva.FindAsync(id);
                if (vm.Reserva == null)
                {
                    return NotFound();
                }
            }

                ListaClientes = _context.Cliente
                    .Where(c => c.Estado == "ACTIVO")
                    .Select(x => new SelectListItem
                    {
                        Text = x.Nombres + " " + x.Apellidos,
                        Value = x.IdCliente.ToString()
                    }).ToList(),

                ListaHabitaciones = _context.Habitacion
                    .Select(x => new SelectListItem
                    {
                        Text = x.Numero,
                        Value = x.IdHabitacion.ToString()
                    }).ToList(),

                ListaTipoHabitacion = _context.Tipo_Habitacion
                    .Select(x => new SelectListItem
                    {
                        Text = x.NombreTipo,
                        Value = x.IdTipoHabitacion.ToString()
                    }).ToList()
            };

            return PartialView("_NuevaReservaPartial", vm);
        }

        // POST: /Reserva/NuevaReserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevaReserva(ReservaViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.Reserva.FechaReserva = DateTime.Now;

                // Asignar recepcionista desde la sesión
                var usuarioSesion = HttpContext.Session.GetString("Usuario");
                var recep = await _context.Recepcionista
                    .FirstOrDefaultAsync(r => r.Usuario == usuarioSesion);

                vm.Reserva.IdRecepcionista = recep?.IdRecepcionista ?? 1;

                // Calcular costo
                var habitacion = await _context.Habitacion
                    .FindAsync(vm.Reserva.IdHabitacion);

                if (habitacion != null)
                {
                    int noches = (vm.Reserva.FechaSalida - vm.Reserva.FechaEntrada).Days;
                    if (noches <= 0) noches = 1;
                    vm.Reserva.CostoTotal = habitacion.Precio * noches;
                }

                _context.Reserva.Add(vm.Reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recargar listas si hay error
            vm.ListaClientes = _context.Cliente
                .Select(x => new SelectListItem
                {
                    Text = x.Nombres + " " + x.Apellidos,
                    Value = x.IdCliente.ToString()
                }).ToList();

            vm.ListaHabitaciones = _context.Habitacion
                .Select(x => new SelectListItem
                {
                    Text = x.Numero,
                    Value = x.IdHabitacion.ToString()
                }).ToList();

            vm.ListaTipoHabitacion = _context.Tipo_Habitacion
                .Select(x => new SelectListItem
                {
                    Text = x.NombreTipo,
                    Value = x.IdTipoHabitacion.ToString()
                }).ToList();

            return PartialView("_NuevaReservaPartial", vm);
        }

        // GET: /Reserva/Detalles/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reserva
                .Include(r => r.Cliente)
                .Include(r => r.Habitacion)
                .Include(r => r.Recepcionista)
                .FirstOrDefaultAsync(r => r.IdReserva == id);

            if (reserva == null) return NotFound();

            return View(reserva);
        }

        // GET: /Reserva/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null) return NotFound();

            ViewBag.Clientes = new SelectList(_context.Cliente, "IdCliente", "Nombres", reserva.IdCliente);
            ViewBag.Habitaciones = new SelectList(_context.Habitacion, "IdHabitacion", "Numero", reserva.IdHabitacion);
            ViewBag.Recepcionistas = new SelectList(_context.Recepcionista, "IdRecepcionista", "Nombres", reserva.IdRecepcionista);

            return View(reserva);
        }

        // POST: /Reserva/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Reserva reserva)
        {
            if (id != reserva.IdReserva) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reserva);
        }

        // GET: /Reserva/Eliminar/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reserva
                .Include(r => r.Cliente)
                .Include(r => r.Habitacion)
                .FirstOrDefaultAsync(r => r.IdReserva == id);

            if (reserva == null) return NotFound();

            return View(reserva);
        }

        // POST: /Reserva/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmed(int id)
        {
            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva != null)
            {
                _context.Reserva.Remove(reserva);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}