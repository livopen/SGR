using SGR.Aplicacion.Autorizacion;
using SGR.Dominio.Usuarios;
using SGR.Aplicacion.Usuarios;
namespace SGR.Infraestructura;

public class AutorizacionService : IAutorizacionService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AutorizacionService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public bool PoseeElPermiso(Guid usuarioId, Permiso permisoRequerido)
    {
        var usuario = _usuarioRepository.ObtenerPorId(usuarioId);
        if (usuario == null) return false;

     
        if (usuario.EsAdministrador()) return true;
        if (usuario.tienePermiso(permisoRequerido))
        {
            return true; 
        }

        if (permisoRequerido == Permiso.ActuacionBaja && usuario.tienePermiso(Permiso.ReclamoBaja))
        {
            return true;
        }

      
        return usuario.tienePermiso(permisoRequerido);
    }
}