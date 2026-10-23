namespace SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Autorizacion;
using SGR.Aplicacion.Comun;
using SGR.Aplicacion.Reclamos;
using SGR.Dominio.Actuaciones;
public class AgregarActuacionUseCase
{
    private readonly IActuacionRepository _actuacionRepo;
    private readonly IAutorizacionService _ServicioAutorizacion;
    private readonly IReclamoRepository _reclamoRepo;
    private readonly ActualizacionEstadoReclamoService _servicioDeCambioDeEstado;

    public AgregarActuacionUseCase(IActuacionRepository actuacionRepo, IReclamoRepository reclamoRepo, IAutorizacionService servicioAutorizacion, 
        ActualizacionEstadoReclamoService servicionDeCambioDeEstado)
    {
        _actuacionRepo = actuacionRepo;
        _reclamoRepo = reclamoRepo;
        _ServicioAutorizacion = servicioAutorizacion;
        _servicioDeCambioDeEstado = servicionDeCambioDeEstado;
    }
    
    public AgregarActuacionResponse Ejecutar(AgregarActuacionRequest request)
    {
       //Parseo los id a guid
       Guid idReclamo = Guid.Parse(request.idReclamo);
       Guid idUsuario = Guid.Parse(request.IdUsuario);
       //verifico que el usuario este autorizado
       if (!_ServicioAutorizacion.PoseeElPermiso(idUsuario, Permiso.ReclamoAlta.ToString()))
            throw new AutorizacionException("No autorizado.");
       //verifico que el reclamo exista 
       var reclamo = _reclamoRepo.ObtenerPorId(idReclamo);
       if(reclamo == null) 
            throw new EntidadNoEncontradaException("No se encontro el reclamo.");
    
       // Verifico que el tipo de acutacion sea correcto
       if (!Enum.TryParse<TipoActuacion>(request.tipo, true, out var tipoNuevoValido))
        {
            throw new AplicacionException($"El tipo'{request.tipo}' no es válido.");
        }        
       //creo el value object
       var detalleValido = new DetalleActuacion(request.detalle);
       
       var nuevaActuacion = new Actuacion(idReclamo,tipoNuevoValido,detalleValido,idUsuario);
       
       _actuacionRepo.Agregar(nuevaActuacion);

       //llamo al servicio de actualizacion de estado
       _servicioDeCambioDeEstado.Ejecutar(reclamo,idUsuario); 
       
       return new AgregarActuacionResponse(nuevaActuacion.Id.ToString(),nuevaActuacion.Detalle.Descripcion);
    }
}

public record AgregarActuacionRequest(string idReclamo, string tipo, string detalle, string IdUsuario);
public record AgregarActuacionResponse(string IdReclamo, string descripcion);