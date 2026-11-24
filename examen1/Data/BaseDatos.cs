using examen1.Models;
using Microsoft.EntityFrameworkCore;

namespace examen1.Data
{
    public class BaseDatos : DbContext
    {
        public BaseDatos(DbContextOptions<BaseDatos> options) : base(options) { }

        public DbSet<Jefe> Jefes { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Jefe>()
                .HasIndex(j => j.Nombre)
                .IsUnique();

            modelBuilder.Entity<Proyecto>()
                .HasIndex(p => p.Nombre)
                .IsUnique();

            modelBuilder.Entity<Proyecto>()
                .HasIndex(p => p.IdJefe)
                .IsUnique();
        }
    }
}