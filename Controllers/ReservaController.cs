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
        public IActionResult Index()
        {
            var listaReservas = _context.Reserva
                .Include(r => r.IdCliente)
                .Include(r => r.IdHabitacion)
                .ToList();

            return View(listaReservas);
        }

        public IActionResult NuevaReserva()
        {
            ReservaViewModel vm = new ReservaViewModel()
            {
                Reserva = new Reserva(),
                Cliente = new Cliente(),
                Habitacion = new Habitacion(),

                ListaClientes = _context.Cliente
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
    }
}