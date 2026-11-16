using SGR.Dominio.Usuarios;
namespace SGR.Aplicacion.Autorizacion;

public interface IAutorizacionService
{
    bool PoseeElPermiso(Guid idUsuario,Permiso permiso);
}
