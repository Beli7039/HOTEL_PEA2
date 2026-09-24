using System.ComponentModel.DataAnnotations;

namespace HOTEL_PEA2.Models
{
    public class Recepcionista
    {
        [Key]
        public int IdRecepcionista { get; set; }

        public string Nombres { get; set; }

        public string Apellidos { get; set; }

        public string Usuario { get; set; }

        public string Contrasena { get; set; }

        public string Telefono { get; set; }

        public string Email { get; set; }

        public string Estado { get; set; }
    }
}