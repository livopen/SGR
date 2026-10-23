namespace SGR.Aplicacion.Reclamos;
using SGR.Dominio.Reclamos;
using SGR.Aplicacion.Comun;
using SGR.Aplicacion.Autorizacion;
public class CambiarEstadoReclamoUseCase
{
    private readonly IReclamoRepository _reclamoRepo;
    private readonly IAutorizacionService _servicioAutorizacion;
    public CambiarEstadoReclamoUseCase(IReclamoRepository reclamoRepository, IAutorizacionService autorizacionService)
    {
        _reclamoRepo = reclamoRepository;
        _servicioAutorizacion = autorizacionService;
    }
    public CambiarEstadoReclamoResponse Ejecutar(CambiarEstadoReclamoRequest request)
    {
        // 1. Validar permisos
        if (!_servicioAutorizacion.PoseeElPermiso(Guid.Parse(request.IdUsuario), Permiso.ReclamoModificacion.ToString()))
        {
            throw new AutorizacionException("El usuario no tiene permisos para modificar el Reclamo.");
        }

        // 2. Buscar la entidad usando el repositorio del constructor primario
        var reclamo = _reclamoRepo.ObtenerPorId(Guid.Parse(request.reclamoId))
            ?? throw new EntidadNoEncontradaException("No se encontró el reclamo con el ID provisto.");

        // Verifico que el estado nuevo sea correcto
        if (!Enum.TryParse<EstadoReclamo>(request.NuevoEstado, true, out var estadoNuevoValido))
        {
            throw new AplicacionException($"El estado'{request.NuevoEstado}' no es válido.");
        }

        // 3. Ejecuta logica del dominio para cambiar el estado
        reclamo.CambiarEstado(estadoNuevoValido,Guid.Parse(request.IdUsuario));

        // 4. Persistir en la infraestructura
        _reclamoRepo.Modificar(reclamo);

        // 5. Retornamos la respuesta
        return new CambiarEstadoReclamoResponse(estadoNuevoValido.ToString());
    }
}
public record CambiarEstadoReclamoRequest(string IdUsuario, string reclamoId, string NuevoEstado);
public record CambiarEstadoReclamoResponse(string NuevoEstado);