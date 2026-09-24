using GarrasFitnessAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace GarrasFitnessAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Socio> Socios { get; set; }
        public DbSet<Plan> Planes { get; set; }
        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<Pago> Pagos { get; set; }
    }
}