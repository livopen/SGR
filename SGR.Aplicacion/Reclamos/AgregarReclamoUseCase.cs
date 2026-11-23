namespace SGR.Aplicacion.Reclamos;
using SGR.Dominio.Reclamos;
using SGR.Aplicacion.Autorizacion;
using SGR.Dominio.Usuarios;
public class AgregarReclamoUseCase
{
    private readonly IReclamoRepository _repositorio;
    private readonly IAutorizacionService _ServicioAutorizacion;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public AgregarReclamoUseCase(IReclamoRepository repositorio, IAutorizacionService servicioAutorizacion, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _repositorio = repositorio;
        _ServicioAutorizacion = servicioAutorizacion;
        _unidadDeTrabajo = unidadDeTrabajo;
    }
    
    public AgregarReclamoResponse Ejecutar(AgregarReclamoRequest request)
    {
        // 1. Control de autorización
       if (!_ServicioAutorizacion.PoseeElPermiso(Guid.Parse(request.IdUsuario), Permiso.ReclamoAlta))
            throw new AutorizacionException("No autorizado.");

        // 2. Creación del objeto de negocio
        var asuntoNuevo = new Asunto(request.asunto); 
        var reclamoNuevo = new Reclamo(asuntoNuevo,Guid.Parse(request.IdUsuario));

        // 3. El repositorio local "registra" el Reclamo 
        _repositorio.Agregar(reclamoNuevo);
        _unidadDeTrabajo.Guardar();
        
        // 4. Retornamos la respuesta
        return new AgregarReclamoResponse(reclamoNuevo.Id.ToString(), reclamoNuevo.Asunto.Descripcion , DateTime.Now);
    }
}

public record AgregarReclamoRequest(string asunto, string IdUsuario);
public record AgregarReclamoResponse(string IdReclamo, string asunto, DateTime FechaCreacion);