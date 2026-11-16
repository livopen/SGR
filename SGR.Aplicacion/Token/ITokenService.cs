using SGR.Dominio.Usuarios;

namespace SGR.Aplicacion.Token;

public interface ITokenService
{
        string GenerarToken(Usuario usuario);
}