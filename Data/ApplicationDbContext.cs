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
    }
}