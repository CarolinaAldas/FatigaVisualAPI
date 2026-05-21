namespace FatigaVisualAPI.Models;

public class Estadistica
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public int PausasCompletadas { get; set; }
    public decimal HorasPantalla { get; set; }
    public int RachaDias { get; set; }
}