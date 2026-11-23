using SGR.Dominio.Comun;
using SGR.Aplicacion.GestorDeContraseñas;
namespace SGR.Aplicacion.Usuarios;
public class ModificarDatosUsuarioUseCase
{
    private readonly IUsuarioRepository _repository;
    private readonly IPasswordHasher _hasher;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public ModificarDatosUsuarioUseCase(IUsuarioRepository repository, IPasswordHasher hasher,IUnidadDeTrabajo unidadDeTrabajo)
    {
        _repository = repository;
        _hasher = hasher;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public void Ejecutar(ModificarDatosUsuarioRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
            
        if (request.UserIdDesdeToken != request.UserIdAModificar)
            throw new DominioException("No posee autorización para modificar los datos de otro usuario.");

        var usuario = _repository.ObtenerPorId(request.UserIdAModificar);
        if (usuario == null)
            throw new DominioException("Usuario no encontrado.");

        usuario.ActualizarDatos(request.NuevoNombre, request.NuevoCorreo);
        
        if (!string.IsNullOrWhiteSpace(request.NuevacontraseniaPura))
        {
            string nuevoHash = _hasher.HashPassword(request.NuevacontraseniaPura);
            usuario.Cambiarcontrasenia(nuevoHash);
        }

        _repository.Modificar(usuario);
        _unidadDeTrabajo.Guardar();
    }
}

public record ModificarDatosUsuarioRequest(
    Guid UserIdDesdeToken, 
    Guid UserIdAModificar, 
    string NuevoNombre, 
    string NuevoCorreo, 
    string NuevacontraseniaPura
);