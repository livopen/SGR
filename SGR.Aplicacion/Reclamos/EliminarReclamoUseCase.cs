namespace SGR.Aplicacion.Reclamos;
using SGR.Aplicacion.Autorizacion;
using SGR.Dominio.Actuaciones;
using SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Comun;
public class EliminarReclamoUseCase
{
    private readonly IReclamoRepository _reclamoRepo;
    private readonly IAutorizacionService _servicioAutorizacion;
    private readonly IActuacionRepository _actuacionRepo;

    public EliminarReclamoResponse Ejecutar(EliminarReclamoRequest request)
    {
        // 1. Validación de Seguridad
        if (!_servicioAutorizacion.PoseeElPermiso(Guid.Parse(request.UsuarioId), Permiso.ReclamoBaja.ToString()))
        {
            throw new AutorizacionException("No tenés permisos para eliminar reclamos.");
        }

        // 2. Validación de Existencia 
        // Buscamos el reclamo
        var reclamo = _reclamoRepo.ObtenerPorId(Guid.Parse(request.IdReclamo))
            ?? throw new EntidadNoEncontradaException($"El reclamo con ID {request.IdReclamo} no existe.");
        //Si existe el reclamo, busco todas las actuaciones con el id del reclamo
        IEnumerable<Actuacion> lista= _actuacionRepo.ObtenerActuacionesIdReclamo(Guid.Parse(request.IdReclamo)).ToList(); 
        
        // 3. Elimino todas las actuaciones
        foreach(Actuacion a in lista)
        {
            _actuacionRepo.Eliminar(a.Id);
        }
        // 4 Elimino el reclamo
        _reclamoRepo.Eliminar(Guid.Parse(request.IdReclamo));

        // 5. Retorno del DTO Response
        return new EliminarReclamoResponse(Exito: true, IdReclamoEliminado: request.IdReclamo);
    }
}

public record EliminarReclamoRequest(string IdReclamo, string UsuarioId);
public record EliminarReclamoResponse(bool Exito, string IdReclamoEliminado);