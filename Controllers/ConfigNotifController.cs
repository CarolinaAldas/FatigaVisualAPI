using FatigaVisualAPI.Data;
using FatigaVisualAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FatigaVisualAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigNotifController(AppDbContext db) : ControllerBase
{
    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<IEnumerable<ConfigNotificacion>>> GetByUsuario(int usuarioId)
        => Ok(await db.ConfigNotif
            .Where(c => c.UsuarioId == usuarioId)
            .ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<ConfigNotificacion>> GetById(int id)
    {
        var c = await db.ConfigNotif.FindAsync(id);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public async Task<ActionResult<ConfigNotificacion>> Create(ConfigNotificacion config)
    {
        db.ConfigNotif.Add(config);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = config.Id }, config);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ConfigNotificacion config)
    {
        if (id != config.Id) return BadRequest();
        db.Entry(config).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> Toggle(int id)
    {
        var c = await db.ConfigNotif.FindAsync(id);
        if (c is null) return NotFound();
        c.Activa = !c.Activa;
        await db.SaveChangesAsync();
        return Ok(new { id = c.Id, activa = c.Activa });
    }
}