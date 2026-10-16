namespace SGR.Aplicacion.Reclamos;
using SGR.Aplicacion.Reclamos;
using SGR.Aplicacion.Autorizacion;
using SGR.Aplicacion.Comun;
using SGR.Dominio.Reclamos;
public class ModificarAsuntoReclamoUseCase
{
    private readonly IReclamoRepository _reclamoRepo;
    private readonly IAutorizacionService _autorizacionService;
    public ModificarAsuntoReclamoResponse Ejecutar(ModificarAsuntoReclamoRequest request)
    {
        // 1. Validar permisos 
        if (!_autorizacionService.PoseeElPermiso(Guid.Parse(request.UsuarioId), Permiso.ReclamoModificacion.ToString()))
        {
            throw new AutorizacionException($"El usuario {request.UsuarioId} no tiene permisos para modificar Reclamos.");
        }
        // 2. Buscar la entidad 
        var reclamo = _reclamoRepo.ObtenerPorId(Guid.Parse(request.ReclamoId)) 
            ?? throw new EntidadNoEncontradaException($"No se encontró el reclamo con ID {request.ReclamoId}");
        // 3. Crear el Value Object
        var nuevoAsunto = new Asunto(request.descripcion);
        // 4. El dominio cambia el asunto
        reclamo.ModificarAsunto(nuevoAsunto,Guid.Parse(request.UsuarioId));
        // 5. Persistir los cambios
        _reclamoRepo.Modificar(reclamo); 
        // 6. Retornar la respuesta 
        return new ModificarAsuntoReclamoResponse(reclamo.Id.ToString(),reclamo.Asunto.Descripcion.ToString());
    }
}
public record ModificarAsuntoReclamoResponse(string Id, string descripcion);
public record ModificarAsuntoReclamoRequest(string descripcion, string UsuarioId, string ReclamoId);