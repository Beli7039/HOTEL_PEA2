using System.ComponentModel.DataAnnotations;

namespace HOTEL_PEA2.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        public string Dni { get; set; }

        public string Nombres { get; set; }

        public string Apellidos { get; set; }

        public string Telefono { get; set; }

        public string Email { get; set; }

        public DateTime FechaRegistro { get; set; }

        public bool Estado { get; set; }
    }
}