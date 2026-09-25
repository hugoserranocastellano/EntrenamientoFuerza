using EntrenamientoFuerza.Data;
using EntrenamientoFuerza.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EntrenamientoFuerza.Services;

// Autenticacion propia con usuario/contraseña hasheada (PBKDF2 via el hasher de ASP.NET Identity,
// sin tirar de todo el sistema de Identity).
public class UsuarioAuth(IDbContextFactory<ApplicationDbContext> dbFactory)
{
    private readonly PasswordHasher<Usuario> _hasher = new();

    public async Task<Usuario?> ValidarAsync(string nombreUsuario, string password)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var usuario = await db.Usuarios
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);

        if (usuario is null)
            return null;

        var resultado = _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, password);
        return resultado == PasswordVerificationResult.Failed ? null : usuario;
    }

    public string HashPassword(Usuario usuario, string password) => _hasher.HashPassword(usuario, password);
}
