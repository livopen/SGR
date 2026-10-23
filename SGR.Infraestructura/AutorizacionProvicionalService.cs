using SGR.Aplicacion.Autorizacion;

namespace SGR.Infraestructura.Autorizacion;

public sealed class AutorizacionProvisionalService : IAutorizacionService
{
    public bool PoseeElPermiso(Guid idUsuario, string permiso)
    {
        return true;
    }
}
