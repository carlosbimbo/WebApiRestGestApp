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
        public DbSet<RegistroEvento> RegistroEventos { get; set; }
        public DbSet<EtapaGestacional> EtapasGestacionales { get; set; }
        public DbSet<RegistroSuplemento> RegistroSuplementos { get; set; }
        public DbSet<RegistroHemoglobina> RegistroHemoglobinas { get; set; }
        public DbSet<DiasGestacion> DiasGestaciones { get; set; }
        public DbSet<AgendaGestacional> AgendasGestacionales { get; set; }        
        public DbSet<AlarmExecution> AlarmExecutions { get; set; }
        public DbSet<ValoracionRuletaMenu> ValoracionRuletaMenus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para Users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Configuración Exclusiva para Eventos (Llave Compuesta)
            modelBuilder.Entity<RegistroEvento>()
                .HasKey(e => new { e.IdEven, e.IdUser });

            // Configuración para Suplementos (Llave Compuesta)
            modelBuilder.Entity<RegistroSuplemento>()
                .HasKey(s => new { s.IdSuple, s.IdUser });

            // Configuración para Hemoglobina (Llave Compuesta)
            modelBuilder.Entity<RegistroHemoglobina>()
                .HasKey(h => new { h.Idh, h.IdUser });

            // Configuración para Días de Gestación (Llave Compuesta)
            modelBuilder.Entity<DiasGestacion>()
                .HasKey(d => new { d.IdDiasg, d.IdUser });

            // Configuración para Agenda (Llave Compuesta)
            modelBuilder.Entity<AgendaGestacional>()
                .HasKey(a => new { a.Id, a.Nrosem });

            // Configuración para AlarmExecution (Índice único compuesto anti-duplicados)
            modelBuilder.Entity<AlarmExecution>()
                .HasIndex(a => new { a.UserId, a.IdAlar, a.Fecha })
                .IsUnique()
                .HasDatabaseName("ux_alarm_unique");

            modelBuilder.Entity<ValoracionRuletaMenu>()
                .HasKey(v => new { v.IdUser, v.IdMenu });
        }
    }
}