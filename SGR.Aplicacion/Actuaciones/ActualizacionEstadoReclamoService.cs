namespace SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Reclamos;
using SGR.Dominio.Reclamos;
using SGR.Aplicacion.Comun;
using SGR.Dominio.Actuaciones;

public class ActualizacionEstadoReclamoService
{
    private readonly IReclamoRepository _reclamoRepo;
    private readonly IActuacionRepository _actuacionRepo; 

    public ActualizacionEstadoReclamoService(IReclamoRepository reclamoRepo, IActuacionRepository actuacionRepo)
    {
        _reclamoRepo = reclamoRepo;
        _actuacionRepo = actuacionRepo;
    }

    public bool Ejecutar(Reclamo reclamo, Guid idUsuario)
    {
        if (reclamo == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el reclamo {reclamo}");
        }
        
        var actuacionesDelReclamo = this._actuacionRepo.ObtenerActuacionesIdReclamo(reclamo.Id)
            .Where(t => t.Id == reclamo.Id).ToList();
        // Determinamos cuál es el "último" basándonos en la FechaCreacion más reciente
        var ultimaActuacion = actuacionesDelReclamo
            .OrderByDescending(t => t.FechaCreacion)
            .FirstOrDefault();
        // Extraemos el tipo de actuacion. Si no tiene actuaciones (porque se eliminaron), queda en null
        TipoActuacion? ultimoTipo = ultimaActuacion?.Tipo;
        // Le pedimos a la entidad que evalúe y aplique la regla de negocio
        bool huboCambio = reclamo.ActualizarEstado(ultimoTipo,idUsuario);
        // Solo si mutó el estado, guardamos el reclamo modificado en el archivo
        if (huboCambio)
        {
            _reclamoRepo.Modificar(reclamo);
        }
        
        return huboCambio;
    }
}
