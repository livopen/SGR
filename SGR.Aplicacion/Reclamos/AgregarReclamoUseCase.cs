namespace SGR.Aplicacion.Reclamos;
using SGR.Dominio.Reclamos;
using SGR.Aplicacion.Autorizacion;
public class AgregarReclamoUseCase
{
    private readonly IReclamoRepository _repositorio;
    private readonly IAutorizacionService _ServicioAutorizacion;

    public AgregarReclamoUseCase(IReclamoRepository repositorio, IAutorizacionService servicioAutorizacion)
    {
        _repositorio = repositorio;
        _ServicioAutorizacion = servicioAutorizacion;
    }
    
    public AgregarReclamoResponse Ejecutar(AgregarReclamoRequest request)
    {
        // 1. Control de autorización
       if (!_ServicioAutorizacion.PoseeElPermiso(request.IdUsuario, Permiso.ReclamoAlta.ToString()))
            throw new AutorizacionException("No autorizado.");

        // 2. Creación del objeto de negocio
        var asuntoNuevo = new Asunto(request.asunto); 
        var reclamoNuevo = new Reclamo(asuntoNuevo, request.IdUsuario);

        // 3. El repositorio local "registra" el Reclamo 
        _repositorio.Agregar(reclamoNuevo);
        
        // 4. Retornamos la respuesta
        return new AgregarReclamoResponse(reclamoNuevo.Id, reclamoNuevo.Asunto.Descripcion , DateTime.Now);
    }
}

public record AgregarReclamoRequest(string asunto, Guid IdUsuario);
public record AgregarReclamoResponse(Guid IdReclamo, string asunto, DateTime FechaCreacion);