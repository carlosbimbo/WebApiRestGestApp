using Microsoft.EntityFrameworkCore;
using WebApiRestGestApp.Models;

namespace WebApiRestGestApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        // Agregamos la nueva tabla al contexto
        public DbSet<RegistroEvento> RegistroEventos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para Users (Mantenemos lo que ya tenías)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Configuración Exclusiva para Eventos (Llave Compuesta)
            modelBuilder.Entity<RegistroEvento>()
                .HasKey(e => new { e.IdEven, e.IdUser });
        }
    }
}