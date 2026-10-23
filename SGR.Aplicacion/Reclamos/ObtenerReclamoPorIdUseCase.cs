namespace SGR.Aplicacion.Reclamos;

using SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Comun;
using SGR.Dominio.Actuaciones;

public class ObtenerReclamoPorIdUseCase
{
    private readonly IReclamoRepository _reclamoRepo;
    private readonly IActuacionRepository _actuacionRepo;
    public ObtenerReclamoPorIdResponse Ejecutar(ObtenerReclamoPorIdRequest request)
    {
        var reclamo = _reclamoRepo.ObtenerPorId(Guid.Parse(request.IdReclamo));
        if (reclamo == null)
        {
        throw new EntidadNoEncontradaException($"El reclamo con ID {request.IdReclamo} no existe.");
        }

        IEnumerable<Actuacion> listaActuaciones = _actuacionRepo.ObtenerActuacionesIdReclamo(Guid.Parse(request.IdReclamo));
        // Mapear la lista de actuaciones de 'actuacion' a 'actuacionDTO'
        var listaActuacionesDTO = listaActuaciones.Select(t => new ActuacionDTO(
            t.Id.ToString(), 
            t.Tipo.ToString(), 
            t.Detalle.ToString(), 
            t.FechaCreacion
        )).ToList();

        return new ObtenerReclamoPorIdResponse(
            reclamo.Id.ToString(),
            reclamo.Asunto.Descripcion.ToString(), 
            reclamo.Estado.ToString(),
            listaActuacionesDTO
        );
    }
}

public record ObtenerReclamoPorIdRequest(string IdReclamo);
public record ObtenerReclamoPorIdResponse(string IdReclamo, string asunto, string Estado, List<ActuacionDTO> actuaciones);
public record ActuacionDTO(string idActuacion, string tipo, string detalle, DateTime fechaCreacion);