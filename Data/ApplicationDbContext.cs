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
            // ============================================================
            // NOMBRES DE TABLAS
            // Se mantienen los nombres exactos que ya usa la BD.
            // ============================================================
            modelBuilder.Entity<Cliente>().ToTable("Cliente");
            modelBuilder.Entity<Habitacion>().ToTable("Habitacion");
            modelBuilder.Entity<Recepcionista>().ToTable("Recepcionista");
            modelBuilder.Entity<Reserva>().ToTable("Reserva");
            modelBuilder.Entity<Tipo_Habitacion>().ToTable("Tipo_Habitacion");
            modelBuilder.Entity<Usuario>().ToTable("Usuario");

            // ============================================================
            // ÍNDICES ÚNICOS (evitan datos duplicados)
            // ============================================================

            // DNI único en Cliente
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Dni)
                .IsUnique();

            // UserName único en Usuario
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            // Numero único en Habitacion
            modelBuilder.Entity<Habitacion>()
                .HasIndex(h => h.Numero)
                .IsUnique();

            // NombreTipo único en Tipo_Habitacion
            modelBuilder.Entity<Tipo_Habitacion>()
                .HasIndex(t => t.NombreTipo)
                .IsUnique();

            // ============================================================
            // PRECISIÓN DE DECIMALES
            // Se define explícitamente decimal(18,2) para evitar
            // advertencias de EF Core y garantizar que no se trunquen
            // los valores. 18 dígitos totales, 2 decimales.
            // ============================================================
            modelBuilder.Entity<Habitacion>()
                .Property(h => h.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Tipo_Habitacion>()
                .Property(t => t.PrecioBase)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reserva>()
                .Property(r => r.CostoTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reserva>()
                .Property(r => r.CargoAdministrativo)
                .HasPrecision(18, 2);

            // ============================================================
            // RELACIONES CON DELETE RESTRICT
            // Evita borrar un registro que tenga dependencias.
            // ============================================================

            // Reserva -> Cliente
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Cliente)
                .WithMany()
                .HasForeignKey(r => r.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            // Reserva -> Habitacion
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Habitacion)
                .WithMany()
                .HasForeignKey(r => r.IdHabitacion)
                .OnDelete(DeleteBehavior.Restrict);

            // Reserva -> Recepcionista
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Recepcionista)
                .WithMany()
                .HasForeignKey(r => r.IdRecepcionista)
                .OnDelete(DeleteBehavior.Restrict);

            // Habitacion -> Tipo_Habitacion
            modelBuilder.Entity<Habitacion>()
                .HasOne(h => h.TipoHabitacion)
                .WithMany()
                .HasForeignKey(h => h.IdTipoHabitacion)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}