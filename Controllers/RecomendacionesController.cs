using FatigaVisualAPI.Data;
using FatigaVisualAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FatigaVisualAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecomendacionesController(AppDbContext db) : ControllerBase
{
    [HttpGet("evaluacion/{evaluacionId}")]
    public async Task<ActionResult<IEnumerable<Recomendacion>>> GetByEvaluacion(int evaluacionId)
        => Ok(await db.Recomendaciones
            .Where(r => r.EvaluacionId == evaluacionId)
            .OrderBy(r => r.Prioridad)
            .ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Recomendacion>> GetById(int id)
    {
        var r = await db.Recomendaciones.FindAsync(id);
        return r is null ? NotFound() : Ok(r);
    }

    [HttpPost]
    public async Task<ActionResult<Recomendacion>> Create(Recomendacion recomendacion)
    {
        db.Recomendaciones.Add(recomendacion);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = recomendacion.Id }, recomendacion);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await db.Recomendaciones.FindAsync(id);
        if (r is null) return NotFound();
        db.Recomendaciones.Remove(r);
        await db.SaveChangesAsync();
        return NoContent();
    }
}