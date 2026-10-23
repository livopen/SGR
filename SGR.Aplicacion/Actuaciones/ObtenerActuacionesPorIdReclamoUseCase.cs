namespace SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Reclamos;
using SGR.Aplicacion.Comun;
using SGR.Dominio.Actuaciones;

public class ObtenerActuacionesPorIdReclamoUseCase{
    private IActuacionRepository _actuacionRepo;
    private IReclamoRepository _reclamoRepo;

    public ObtenerActuacionesPorIdReclamoUseCase(IActuacionRepository actuacionRepository, IReclamoRepository reclamoRepository)
    {
        _actuacionRepo = actuacionRepository;
        _reclamoRepo = reclamoRepository;

    }
    public  ObtenerActuacionesPorIdReclamoResponse Ejecutar(ObtenerActuacionesPorIdReclamoRequest request)
    {
        Guid IdReclamo = Guid.Parse(request.idReclamo);
        var reclamo = _reclamoRepo.ObtenerPorId(IdReclamo)
            ?? throw new EntidadNoEncontradaException($"El reclamo con ID {request.idReclamo} no existe.");

        IEnumerable<Actuacion> actuacionesDelReclamo = _actuacionRepo.ObtenerActuacionesIdReclamo(IdReclamo);

        var listaDTO = actuacionesDelReclamo.Select(t => new ActuacionDTO(
            Id: t.Id.ToString(),
            Detalle: t.Detalle.ToString(), 
            tipo: t.Tipo.ToString(),
            FechaCreacion: t.FechaCreacion
        )).ToList();

        return new ObtenerActuacionesPorIdReclamoResponse(listaDTO);
    }
}

public record ObtenerActuacionesPorIdReclamoRequest(string idReclamo);
public record ObtenerActuacionesPorIdReclamoResponse(List<ActuacionDTO> Actuaciones);

public record ActuacionDTO(
    string Id, 
    string Detalle, 
    string tipo, 
    DateTime FechaCreacion
);