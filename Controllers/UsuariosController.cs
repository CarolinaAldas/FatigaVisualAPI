using FatigaVisualAPI.Data;
using FatigaVisualAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FatigaVisualAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetAll()
        => Ok(await db.Usuarios.ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> GetById(int id)
    {
        var u = await db.Usuarios.FindAsync(id);
        return u is null ? NotFound() : Ok(u);
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> Create(Usuario usuario)
    {
        usuario.FechaRegistro = DateTime.UtcNow;
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Usuario usuario)
    {
        if (id != usuario.Id) return BadRequest();
        db.Entry(usuario).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var u = await db.Usuarios.FindAsync(id);
        if (u is null) return NotFound();
        db.Usuarios.Remove(u);
        await db.SaveChangesAsync();
        return NoContent();
    }
}