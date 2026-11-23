using SGR.Aplicacion.Usuarios;
using SGR.Aplicacion.Autorizacion;
using SGR.Dominio.Usuarios;
namespace SGR.Aplicacion;
public class ListarUsuariosUseCase
{
    private readonly IUsuarioRepository _repositorio;

    public ListarUsuariosUseCase(IUsuarioRepository repository)
    {
        _repositorio = repository;
    }
    public ListarUsuariosResponse  Ejecutar(ListarUsuariosRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        var operador = _repositorio.ObtenerPorId(request.OperadorId);
        if (operador == null || !operador._esAdministrador)
            throw new AutorizacionException("Acción denegada: Se requieren privilegios de administrador.");

        var usuarios = _repositorio.ObtenerTodos();
        var listaDtos = usuarios.Select(u => 
        {
            var permisosEfectivos = u._esAdministrador 
                ? Enum.GetValues<Permiso>().ToList() 
                : u.Permisos.ToList();

            return new UsuarioResponseDto(
                u._id, 
                u._nombre, 
                u._correoElectronico.Valor?.ToString() ?? string.Empty, 
                u._esAdministrador,
                permisosEfectivos
            );
        }).ToList();

        return new ListarUsuariosResponse(listaDtos);
    }
}

public record ListarUsuariosRequest(Guid OperadorId);

public record UsuarioResponseDto(Guid Id, string Nombre, string CorreoElectronico, bool EsAdministrador,IReadOnlyCollection<Permiso> Permisos);

public record ListarUsuariosResponse(List<UsuarioResponseDto> Usuarios);