using SGR.Dominio.Comun;
using SGR.Dominio.Usuarios;
using SGR.Aplicacion.GestorDeContraseñas;

namespace SGR.Aplicacion.Usuarios;

public class RegistrarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public RegistrarUsuarioUseCase(
        IUsuarioRepository usuarioRepository, 
        IPasswordHasher passwordHasher,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public RegistrarUsuarioResponse Ejecutar(RegistrarUsuarioRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.CorreoElectronico))
        {
            throw new ArgumentException("El correo electrónico es obligatorio.");
        }

        string correoNormalizado = request.CorreoElectronico.Trim().ToLower();
    
        var usuarioExistente = _usuarioRepository.ObtenerPorCorreo(new CorreoElectronico(correoNormalizado));
        if (usuarioExistente is not null)
        {
            throw new DominioException("El correo electrónico ya se encuentra registrado.");
        }


        ///Este if evita registrar usuarios con contraseñas vacias
        if (string.IsNullOrWhiteSpace(request.contrasenia))
        {
            throw new ArgumentException("La contraseña es obligatoria.");
        }


        string hashcontrasenia = _passwordHasher.HashPassword(request.contrasenia);

        var correo = new CorreoElectronico(correoNormalizado);

        var nuevoUsuario = new Usuario(
            request.Nombre ?? "Usuario sin nombre", 
            correo, 
            hashcontrasenia, 
            esAdministrador: false 
        );

        _usuarioRepository.Agregar(nuevoUsuario);

        _unidadDeTrabajo.Guardar(); 

        return new RegistrarUsuarioResponse(nuevoUsuario._id,nuevoUsuario._nombre!,nuevoUsuario._correoElectronico.Valor!,"Usuario registrado exitosamente.");
    }
}

public record class RegistrarUsuarioRequest(string? Nombre,string CorreoElectronico,string contrasenia,bool EsAdministrador);
public record class RegistrarUsuarioResponse(Guid Id,string Nombre,string CorreoElectronico,string Mensaje);