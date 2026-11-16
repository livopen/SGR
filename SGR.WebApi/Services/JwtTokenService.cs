using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SGR.Aplicacion.Token;
using SGR.Dominio.Usuarios;

namespace SGR.WebApi.Services;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerarToken(Usuario usuario)
    {
        var secretKey = _configuration["Jwt:Key"] 
            ?? throw new InvalidOperationException("La clave secreta de JWT no está configurada en appsettings.json.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Claims estándares con los datos del usuario autenticado
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario._id.ToString()),
            new(ClaimTypes.Name, usuario._nombre ?? throw new InvalidOperationException("El usuario no tiene un nombre configurado.")),
            new(ClaimTypes.Email, usuario._correoElectronico?.Valor ?? throw new InvalidOperationException("El usuario no tiene un correo electrónico configurado.")),
            new(ClaimTypes.Role, usuario._esAdministrador ? "Administrador" : "Usuario")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8), // Expiración configurable de 8 horas
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}