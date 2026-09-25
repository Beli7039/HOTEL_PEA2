using Microsoft.EntityFrameworkCore;
using HOTEL_PEA2.Models;

namespace HOTEL_PEA2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuario { get; set; }

        public DbSet<Cliente> Cliente { get; set; }

        public DbSet<Habitacion> Habitacion { get; set; }

        public DbSet<Tipo_Habitacion> Tipo_Habitacion { get; set; }

        public DbSet<Reserva> Reserva { get; set; }

        public DbSet<Recepcionista> Recepcionista { get; set; }
    }
}