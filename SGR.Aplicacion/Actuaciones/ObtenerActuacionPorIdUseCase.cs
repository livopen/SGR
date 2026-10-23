namespace SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Comun;
public class ObtenerActuacionPorIdUseCase
{
    private IActuacionRepository _actuacionRepo;
    public ObtenerActuacionPorIdUseCase(IActuacionRepository actuacionRepo)
    {
        _actuacionRepo = actuacionRepo;
    }
    public ObtenerActuacionPorIdResponse Ejecutar(ObtenerActuacionPorIdRequest request)
    {
        Guid idActuacion = Guid.Parse(request.idActuacion);
        Guid idUsuario = Guid.Parse(request.idUsuario);

        var actuacion = _actuacionRepo.ObtenerTodos().FirstOrDefault(t => t.Id == idActuacion)
            ?? throw new EntidadNoEncontradaException($"La actuacion con ID {idActuacion} no existe.");

        return new ObtenerActuacionPorIdResponse(
            Id: actuacion.Id.ToString(),
            Detalle: actuacion.Detalle.ToString(),
            tipo: actuacion.Tipo.ToString(),
            FechaCreacion: actuacion.FechaCreacion
        );
    }
}

public record ObtenerActuacionPorIdRequest(string idActuacion, string idUsuario);
public record ObtenerActuacionPorIdResponse(string Id, string Detalle, string tipo, DateTime FechaCreacion);
