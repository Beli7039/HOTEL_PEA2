using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOTEL_PEA2.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        public string? Nombre { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Clave { get; set; } = string.Empty;

        public string? Rol { get; set; }

        public bool Estado { get; set; }
    }
}