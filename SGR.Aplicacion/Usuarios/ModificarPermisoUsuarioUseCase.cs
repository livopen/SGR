using SGR.Aplicacion.Autorizacion;
using SGR.Aplicacion.Comun;
using SGR.Dominio.Usuarios;
namespace SGR.Aplicacion.Usuarios;

public class ModificarPermisosUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public ModificarPermisosUsuarioUseCase(IUsuarioRepository usuarioRepository, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _usuarioRepository = usuarioRepository;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public ModificarPermisosResponse Ejecutar(ModificarPermisosRequest request)
    {
        if (request == null) throw new ArgumentException("Request inválido.");

        var operador = _usuarioRepository.ObtenerPorId(request.IdUsuarioOperador);
        if (operador == null || !operador._esAdministrador)
            throw new AutorizacionException("Acción denegada: Se requieren permisos de Administrador.");

        var usuario = _usuarioRepository.ObtenerPorId(request.UsuarioId);
        if (usuario == null)
            throw new CredencialesInvalidasException("El usuario especificado no existe.");

        usuario.ReemplazarPermisos(request.NuevosPermisos);
        _usuarioRepository.Modificar(usuario);
        _unidadDeTrabajo.Guardar();
        return new ModificarPermisosResponse(true,"Permisos actualizados con éxito.");
    }
}
public record class ModificarPermisosRequest(Guid UsuarioId,List<Permiso> NuevosPermisos,Guid IdUsuarioOperador);
public class ModificarPermisosResponse(bool Exitoso, string Mensaje);