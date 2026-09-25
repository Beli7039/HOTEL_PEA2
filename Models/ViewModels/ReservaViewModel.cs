using Microsoft.AspNetCore.Mvc.Rendering;
using HOTEL_PEA2.Models;

namespace HOTEL_PEA2.Models.ViewModels
{
    public class ReservaViewModel
    {
        public Reserva Reserva { get; set; }

        public Cliente Cliente { get; set; }

        public Habitacion Habitacion { get; set; }
        public List<SelectListItem> ListaClientes { get; set; }

        public List<SelectListItem> ListaHabitaciones { get; set; }

        public List<SelectListItem> ListaTipoHabitacion { get; set; }

        public decimal PrecioNoche { get; set; }

        public int CantidadNoches { get; set; }

        public decimal CostoTotal { get; set; }
    }
}