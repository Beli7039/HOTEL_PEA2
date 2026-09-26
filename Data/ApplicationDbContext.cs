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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Nombre exacto de tablas (tu BD usa singular y con guion bajo)
            modelBuilder.Entity<Cliente>().ToTable("Cliente");
            modelBuilder.Entity<Habitacion>().ToTable("Habitacion");
            modelBuilder.Entity<Recepcionista>().ToTable("Recepcionista");
            modelBuilder.Entity<Reserva>().ToTable("Reserva");
            modelBuilder.Entity<Tipo_Habitacion>().ToTable("Tipo_Habitacion");
            modelBuilder.Entity<Usuario>().ToTable("Usuario");

            // DNI único
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Dni)
                .IsUnique();

            // Usuario.UserName único
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            // Relaciones Reserva
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Cliente)
                .WithMany()
                .HasForeignKey(r => r.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Habitacion)
                .WithMany()
                .HasForeignKey(r => r.IdHabitacion)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Recepcionista)
                .WithMany()
                .HasForeignKey(r => r.IdRecepcionista)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Habitacion -> Tipo_Habitacion
            modelBuilder.Entity<Habitacion>()
                .HasOne(h => h.TipoHabitacion)
                .WithMany()
                .HasForeignKey(h => h.IdTipoHabitacion)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}