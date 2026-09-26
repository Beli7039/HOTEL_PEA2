using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOTEL_PEA2.Models
{
    public class Habitacion
    {
        [Key]
        public int IdHabitacion { get; set; }

        public string Numero { get; set; } = string.Empty;

        public string? Piso { get; set; }

        public int Capacidad { get; set; }

        public decimal Precio { get; set; }

        public string? Estado { get; set; }

        public int IdTipoHabitacion { get; set; }

        [ForeignKey("IdTipoHabitacion")]
        public Tipo_Habitacion? TipoHabitacion { get; set; }
    }
}