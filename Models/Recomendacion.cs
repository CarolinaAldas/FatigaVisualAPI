namespace FatigaVisualAPI.Models;

public class Recomendacion
{
    public int Id { get; set; }
    public int EvaluacionId { get; set; }
    public Evaluacion? Evaluacion { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int Prioridad { get; set; }
}