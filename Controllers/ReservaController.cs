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

        public async Task<IActionResult> Index(string criterio)
        {
            var query = _context.Reserva
                .Include(r => r.Cliente)
                .Include(r => r.Habitacion)
                .AsQueryable();

            if (!string.IsNullOrEmpty(criterio))
            {
                query = query.Where(r => r.Cliente != null &&
                    (r.Cliente.Nombres.Contains(criterio) || r.Cliente.Apellidos.Contains(criterio)));
            }

            var listaReservas = await query.ToListAsync();
            return View(listaReservas);
        }


        public async Task<IActionResult> NuevaReserva(int? id)
        {
            ReservaViewModel vm = new ReservaViewModel();

            if (id == null || id == 0)
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

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarReserva(ReservaViewModel vm)
        {
            if (vm.Reserva.IdReserva == 0)
            {
                _context.Reserva.Add(vm.Reserva);
            }
            else
            {

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