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

        public string Estado { get; set; } = string.Empty;

        public string Observaciones { get; set; } = string.Empty;

        // ==========================================
        // FOREIGN KEYS Y PROPIEDADES DE NAVEGACIÓN
        // ==========================================

        public int IdCliente { get; set; }
        [ForeignKey("IdCliente")]
        public virtual Cliente? Cliente { get; set; }

        public int IdHabitacion { get; set; }
        [ForeignKey("IdHabitacion")]
        public virtual Habitacion? Habitacion { get; set; }

        public int IdRecepcionista { get; set; }
        // Si tienes una tabla o modelo Recepcionista, puedes agregarlo igual:
        // [ForeignKey("IdRecepcionista")]
        // public virtual Recepcionista? Recepcionista { get; set; }
    }
}