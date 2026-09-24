using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOTEL_PEA2.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        public string Nombre { get; set; }

        public string UserName { get; set; }

        public string Clave { get; set; }

        public string Rol { get; set; }

        public int Estado { get; set; }
    }
}
