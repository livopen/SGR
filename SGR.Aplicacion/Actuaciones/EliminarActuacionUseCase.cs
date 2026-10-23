namespace SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Autorizacion;
using SGR.Aplicacion.Comun;
using SGR.Aplicacion.Reclamos;

public class EliminarActuacionUseCase
{
    private IActuacionRepository _actuacionRepo;
    private IAutorizacionService _autorizacion;
    private IReclamoRepository _reclamoRepo;
    private ActualizacionEstadoReclamoService _servicioDeCambioDeEstado;
    public EliminarActuacionUseCase(IActuacionRepository actuacionRepo, IReclamoRepository reclamoRepo, IAutorizacionService servicioAutorizacion,
        ActualizacionEstadoReclamoService servicioDeCambioDeEstado)
    {
        _actuacionRepo = actuacionRepo;
        _reclamoRepo = reclamoRepo;
        _autorizacion = servicioAutorizacion;
        _servicioDeCambioDeEstado = servicioDeCambioDeEstado;
    }
    public EliminarActuacionResponse Ejecutar(EliminarActuacionRequest request)
    {
        //parseo los id a guid
        Guid idActuacion = Guid.Parse(request.IdActuacion);
        Guid idUsuario = Guid.Parse(request.UsuarioId);

        if (!_autorizacion.PoseeElPermiso(idUsuario,Permiso.ActuacionBaja.ToString()))
            throw new AutorizacionException("El usuario no tiene permisos para eliminar actuaciones.");

        var actuacion = _actuacionRepo.ObtenerTodos().FirstOrDefault(t => t.Id == idActuacion)
        ?? throw new EntidadNoEncontradaException($"La actuacion con ID {idActuacion} no existe.");

        Guid reclamoIdAsociado = actuacion.Id;
        //Buscamos el expediente completo
        var reclamo = _reclamoRepo.ObtenerPorId(reclamoIdAsociado)
            ?? throw new EntidadNoEncontradaException($"No existe el reclamo asociado con ID {reclamoIdAsociado}");
        
        _actuacionRepo.Eliminar(idActuacion);
        
        // Recalcular el estado del reclamo
        _servicioDeCambioDeEstado.Ejecutar(reclamo,Guid.Parse(request.UsuarioId));
  
        return new EliminarActuacionResponse(true,idActuacion.ToString());
    }
}
public record EliminarActuacionRequest(string IdActuacion, string UsuarioId);
public record EliminarActuacionResponse(bool Exito, string IdActuacionEliminada);