namespace FatigaVisualAPI.Models;

public class ConfigNotificacion
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int IntervaloMinutos { get; set; }
    public bool Activa { get; set; } = true;
}