using System.ComponentModel.DataAnnotations;

namespace HOTEL_PEA2.Models
{
    public class Tipo_Habitacion
    {
        [Key]
        public int IdTipoHabitacion { get; set; }

        public string NombreTipo { get; set; }

        public string Descripcion { get; set; }

        public decimal PrecioBase { get; set; }

        public int Capacidad { get; set; }
    }
}