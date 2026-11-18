using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SGR.Aplicacion.Token;
using SGR.Dominio.Usuarios;

namespace SGR.WebApi.Services;

public class JwtTokenService(IConfiguration config) : ITokenService
{
    public string GenerarToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario._id.ToString()),
            new Claim(ClaimTypes.Name, usuario._nombre ?? string.Empty),
            new Claim(ClaimTypes.Email, usuario._correoElectronico?.Valor ?? string.Empty),
            new Claim("EsAdministrador", usuario._esAdministrador.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}