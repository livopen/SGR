using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SGR.Aplicacion.Token;
using SGR.Dominio.Usuarios;

namespace SGR.WebApi;

public class JwtTokenProvider(IConfiguration config) : ITokenService
{
    public string GenerarToken(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            // 1. Identificador estándar del usuario para .NET
            new Claim(ClaimTypes.NameIdentifier, usuario._id.ToString()),
            
            // 2. Correo electrónico
            new Claim(ClaimTypes.Email, usuario._correoElectronico.Valor),

            // 3. Claim de Administrador (para autorizar según flag booleano o Rol)
            new Claim("esAdministrador", usuario.EsAdministrador().ToString().ToLower()),
            new Claim(ClaimTypes.Role, usuario.EsAdministrador() ? "Administrador" : "Usuario")
        };

        // 4. Si tu clase Usuario tiene una lista/colección de Permisos, los agregamos en el token:
        if (usuario.Permisos != null)
        {
            foreach (var permiso in usuario.Permisos)
            {
                claims.Add(new Claim("permisos", permiso.ToString()));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}