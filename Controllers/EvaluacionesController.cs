using FatigaVisualAPI.Data;
using FatigaVisualAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FatigaVisualAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvaluacionesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Evaluacion>>> GetAll()
        => Ok(await db.Evaluaciones.Include(e => e.Recomendaciones).ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Evaluacion>> GetById(int id)
    {
        var e = await db.Evaluaciones
            .Include(e => e.Recomendaciones)
            .FirstOrDefaultAsync(e => e.Id == id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<IEnumerable<Evaluacion>>> GetByUsuario(int usuarioId)
        => Ok(await db.Evaluaciones
            .Include(e => e.Recomendaciones)
            .Where(e => e.UsuarioId == usuarioId)
            .OrderByDescending(e => e.Fecha)
            .ToListAsync());

    [HttpPost]
    public async Task<ActionResult<Evaluacion>> Create(Evaluacion evaluacion)
    {
        evaluacion.Fecha = DateTime.UtcNow;
        db.Evaluaciones.Add(evaluacion);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = evaluacion.Id }, evaluacion);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await db.Evaluaciones.FindAsync(id);
        if (e is null) return NotFound();
        db.Evaluaciones.Remove(e);
        await db.SaveChangesAsync();
        return NoContent();
    }
}