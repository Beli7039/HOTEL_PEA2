using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOTEL_PEA2.Models
{
    public class Reserva
    {
        [Key]
        public int IdReserva { get; set; }

        public DateTime FechaEntrada { get; set; } = DateTime.Today;

        public DateTime FechaSalida { get; set; } = DateTime.Today;

        public int CantidadPersonas { get; set; }

        public decimal CostoTotal { get; set; }

        public string? Estado { get; set; }

        // PROPIEDAD AGREGADA
        public string? TipoHabitacion { get; set; }

        // FOREIGN KEYS
        public int IdCliente { get; set; }
        public int IdHabitacion { get; set; }
        public int? IdRecepcionista { get; set; }

        // PROPIEDADES DE NAVEGACIÓN
        [ForeignKey("IdCliente")]
        public Cliente? Cliente { get; set; }

        [ForeignKey("IdHabitacion")]
        public Habitacion? Habitacion { get; set; }

        [ForeignKey("IdRecepcionista")]
        public Recepcionista? Recepcionista { get; set; }

        public string? Observaciones { get; set; }
    }
}