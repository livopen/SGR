using SGR.Aplicacion.Autorizacion;
using SGR.Dominio.Comun;
namespace SGR.Aplicacion.Usuarios;
public class EliminarUsuarioUseCase
{
    private readonly IUsuarioRepository _repositorio;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo; 

    public EliminarUsuarioUseCase(IUsuarioRepository repository, IUnidadDeTrabajo unitOfWork)
    {
        _repositorio = repository;   
        _unidadDeTrabajo = unitOfWork; 
    }

    public void Ejecutar(EliminarUsuarioRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var operador = _repositorio.ObtenerPorId(request.OperadorId);
        if (operador == null || !operador._esAdministrador)
            throw new AutorizacionException("Acción denegada: Se requieren privilegios de administrador.");

        if (request.OperadorId == request.UsuarioAEliminar)
            throw new AutorizacionException("Un administrador no puede eliminar su propia cuenta.");

        var usuario = _repositorio.ObtenerPorId(request.UsuarioAEliminar);
        if (usuario == null)
            throw new DominioException("El usuario a eliminar no existe.");

        _repositorio.Eliminar(request.UsuarioAEliminar);

        _unidadDeTrabajo.Guardar(); 
    }
}
public record class EliminarUsuarioRequest(Guid OperadorId, Guid UsuarioAEliminar); 
public record class EliminarUsuarioResponse(bool Exito,string Mensaje);
