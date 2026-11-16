using SGR.Aplicacion.Comun;
namespace SGR.Aplicacion.Usuarios;
public class LoginUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    public LoginUseCase(IUsuarioRepository usuarioRepository,IPasswordHasher passwordHasher,ITokenService tokenService)
    {
        _usuarioRepository= usuarioRepository;
        _passwordHasher=passwordHasher;
        _tokenService=tokenService;
    }
    public LoginResponse Ejecutar(LoginRequest request)
    {
        if(request==null || string.IsNullOrEmpty(request.CorreoElectronico) || string.IsNullOrEmpty(request.contrasenia)){
            throw new ArgumentException (" EL correo y contrasenia son oblgatorios");
        }
        // buscamos el usuario en base de datos por correo electronico normalizando a minisculas como se guardo en el registro
        string correoNormalizado = request.CorreoElectronico.Trim().ToLower();
        var usuario = _usuarioRepository.ObtenerPorCorreo(new Dominio.Usuarios.CorreoElectronico(correoNormalizado));
        
        // si no existe tiramos excepcion 
        if (usuario== null)
        {
            throw new CredencialesInvalidasException (" Credenciales incorrectas ");
        }
        // verificar el hash de la contrasenia para ver si coincide con el de la base de datos
        bool claveValida= _passwordHasher.VerifyPassword(request.contrasenia,usuario._contrasenaHash!);
        if (!claveValida)
        {
            throw new CredencialesInvalidasException(" Credenciales incorrectas" );
        }
        //generar el token 
        if (usuario._contrasenaHash != _passwordHasher.HashPassword(request.contrasenia))
        {
        throw new CredencialesInvalidasException(" Credenciales incorrectas");
        }
        string tokenGenerado= _tokenService.GenerarToken(usuario);
        // retornar el response para la web api
        return new LoginResponse(tokenGenerado,usuario._nombre,usuario._correoElectronico.ToString(),usuario._esAdministrador);
    }
}
public record class LoginRequest(string? CorreoElectronico,string? contrasenia);
public record class LoginResponse(string? Token,string? Nombre,string? CorreoElectronico,bool EsAdministrador);