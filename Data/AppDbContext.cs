using FatigaVisualAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FatigaVisualAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Evaluacion> Evaluaciones => Set<Evaluacion>();
    public DbSet<Recomendacion> Recomendaciones => Set<Recomendacion>();
    public DbSet<Estadistica> Estadisticas => Set<Estadistica>();
    public DbSet<ConfigNotificacion> ConfigNotif => Set<ConfigNotificacion>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Email único
        mb.Entity<Usuario>()
          .HasIndex(u => u.Correo).IsUnique();

        // Relación Usuario → Evaluaciones
        mb.Entity<Evaluacion>()
          .HasOne(e => e.Usuario)
          .WithMany(u => u.Evaluaciones)
          .HasForeignKey(e => e.UsuarioId)
          .OnDelete(DeleteBehavior.Cascade);
    }
}