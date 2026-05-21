using FatigaVisualAPI.Models;
using System.ComponentModel.DataAnnotations;

public class Evaluacion
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public string RespuestasJson { get; set; } = "[]";

    [Range(0, 100)]
    public int IndiceFatiga { get; set; }

    public string Nivel { get; set; } = "bajo";

    public ICollection<Recomendacion> Recomendaciones { get; set; } = [];
}