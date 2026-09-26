using System.ComponentModel.DataAnnotations;

namespace HOTEL_PEA2.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        public string Dni { get; set; } = string.Empty;

        public string Nombres { get; set; } = string.Empty;

        public string Apellidos { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public string? Email { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public string Estado { get; set; } = "ACTIVO";
    }
}