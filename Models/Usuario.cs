using System.ComponentModel.DataAnnotations;

namespace FatigaVisualAPI.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Correo { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; }
        = DateTime.UtcNow;

    // Navegación
    public ICollection<Evaluacion> Evaluaciones { get; set; } = [];
    public ICollection<Estadistica> Estadisticas { get; set; } = [];
    public ICollection<ConfigNotificacion> Notificaciones { get; set; } = [];
}