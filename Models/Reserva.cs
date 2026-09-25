using System.ComponentModel.DataAnnotations;

namespace HOTEL_PEA2.Models
{
    public class Reserva
    {
        [Key]
        public int IdReserva { get; set; }

        public DateTime FechaEntrada { get; set; }

        public DateTime FechaSalida { get; set; }

        public int CantidadPersonas { get; set; }

        public string TipoHabitacion { get; set; } = string.Empty;

        public decimal CostoTotal { get; set; }

        public string Estado { get; set; }

        public string Observaciones { get; set; }

        // FOREIGN KEYS

        public int IdCliente { get; set; }

        public int IdHabitacion { get; set; }

        public int IdRecepcionista { get; set; }
    }
}