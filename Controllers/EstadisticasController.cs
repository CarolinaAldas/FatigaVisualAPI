using FatigaVisualAPI.Data;
using FatigaVisualAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FatigaVisualAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadisticasController(AppDbContext db) : ControllerBase
{
    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<IEnumerable<Estadistica>>> GetByUsuario(int usuarioId)
        => Ok(await db.Estadisticas
            .Where(e => e.UsuarioId == usuarioId)
            .OrderByDescending(e => e.Fecha)
            .ToListAsync());

    [HttpGet("usuario/{usuarioId}/semana")]
    public async Task<ActionResult<IEnumerable<Estadistica>>> GetSemana(int usuarioId)
    {
        var hace7dias = DateTime.UtcNow.AddDays(-7);
        return Ok(await db.Estadisticas
            .Where(e => e.UsuarioId == usuarioId && e.Fecha >= hace7dias)
            .OrderBy(e => e.Fecha)
            .ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Estadistica>> GetById(int id)
    {
        var e = await db.Estadisticas.FindAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<ActionResult<Estadistica>> Create(Estadistica estadistica)
    {
        estadistica.Fecha = DateTime.UtcNow;
        db.Estadisticas.Add(estadistica);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = estadistica.Id }, estadistica);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Estadistica estadistica)
    {
        if (id != estadistica.Id) return BadRequest();
        db.Entry(estadistica).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return NoContent();
    }
}