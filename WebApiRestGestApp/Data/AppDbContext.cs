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
        public DbSet<EtapaGestacional> EtapasGestacionales { get; set; }
        public DbSet<RegistroSuplemento> RegistroSuplementos { get; set; }
        public DbSet<RegistroHemoglobina> RegistroHemoglobinas { get; set; }
        public DbSet<DiasGestacion> DiasGestaciones { get; set; }
        public DbSet<AgendaGestacional> AgendasGestacionales { get; set; }

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

            // Configuración para Suplementos (Llave Compuesta)
            modelBuilder.Entity<RegistroSuplemento>()
                .HasKey(s => new { s.IdSuple, s.IdUser });

            modelBuilder.Entity<RegistroHemoglobina>()
                .HasKey(h => new { h.Idh, h.IdUser });

            modelBuilder.Entity<DiasGestacion>()
                .HasKey(d => new { d.IdDiasg, d.IdUser });

            modelBuilder.Entity<AgendaGestacional>()
                .HasKey(a => new { a.Id, a.Nrosem });

        }
    }
}