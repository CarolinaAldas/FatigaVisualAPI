using FatigaVisualAPI.Data;
using FatigaVisualAPI.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FatigaVisualAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, IConfiguration config) : ControllerBase
{
    // POST api/auth/google
    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            // 1 — Verificar el token con Google
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                request.IdToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [config["Google:ClientId"]]
                });

            // 2 — Buscar o crear el usuario
            var usuario = await db.Usuarios
                .FirstOrDefaultAsync(u => u.GoogleId == payload.Subject
                                       || u.Correo == payload.Email);

            if (usuario is null)
            {
                // Crear nuevo usuario con Google
                usuario = new Usuario
                {
                    Nombre = payload.Name,
                    Correo = payload.Email,
                    GoogleId = payload.Subject,
                    FotoUrl = payload.Picture,
                    FechaRegistro = DateTime.UtcNow
                };
                db.Usuarios.Add(usuario);
                await db.SaveChangesAsync();
            }
            else if (usuario.GoogleId is null)
            {
                // Vincular cuenta existente con Google
                usuario.GoogleId = payload.Subject;
                usuario.FotoUrl = payload.Picture;
                await db.SaveChangesAsync();
            }

            // 3 — Generar JWT
            var jwt = GenerarJwt(usuario);

            return Ok(new
            {
                token = jwt,
                usuario = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Correo,
                    usuario.FotoUrl
                }
            });
        }
        catch (InvalidJwtException)
        {
            return Unauthorized(new { mensaje = "Token de Google inválido" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var usuario = await db.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == request.Correo
                                   && u.PasswordHash == request.Password);

        if (usuario is null)
            return Unauthorized(new { mensaje = "Credenciales incorrectas" });

        var jwt = GenerarJwt(usuario);

        return Ok(new
        {
            token = jwt,
            usuario = new
            {
                usuario.Id,
                usuario.Nombre,
                usuario.Correo,
                usuario.FotoUrl
            }
        });
    }

    // POST api/auth/registro
    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroRequest request)
    {
        if (await db.Usuarios.AnyAsync(u => u.Correo == request.Correo))
            return BadRequest(new { mensaje = "El correo ya está registrado" });

        var usuario = new Usuario
        {
            Nombre = request.Nombre,
            Correo = request.Correo,
            PasswordHash = request.Password,
            FechaRegistro = DateTime.UtcNow
        };

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        var jwt = GenerarJwt(usuario);

        return Ok(new
        {
            token = jwt,
            usuario = new
            {
                usuario.Id,
                usuario.Nombre,
                usuario.Correo
            }
        });
    }

    private string GenerarJwt(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(
                          Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(7);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
            new Claim(JwtRegisteredClaimNames.Name,  usuario.Nombre),
            new Claim("foto", usuario.FotoUrl ?? ""),
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// DTOs
public record GoogleLoginRequest(string IdToken);
public record LoginRequest(string Correo, string Password);
public record RegistroRequest(string Nombre, string Correo, string Password);