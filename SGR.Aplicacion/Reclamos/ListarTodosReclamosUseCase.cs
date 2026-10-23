namespace SGR.Aplicacion.Reclamos;
using SGR.Dominio.Reclamos;
public class ListarTodosReclamosUseCase
{
    private readonly IReclamoRepository _reclamoRepo;
    public ListarTodosReclamosUseCase(IReclamoRepository reclamoRepository)
    {
        _reclamoRepo = reclamoRepository;
    }
    public ListarTodosReclamosResponse Ejecutar(ListarTodosReclamosRequest request)
    {
        // 1. Buscamos todas las entidades del repositorio
        IEnumerable<Reclamo> reclamos = _reclamoRepo.ObtenerTodos();

        // 2. Transformamos las entidades de Dominio a DTOs
        var listaDtos = reclamos.Select(r => new ReclamoDTO(
            Id: r.Id.ToString(),
            Asunto: r.Asunto.ToString(),
            Estado: r.Estado.ToString(),
            FechaCreacion: r.FechaCreacion,
            FechaUltimaModificacion: r.FechaUltimaModificacion
        )).ToList();

        return new ListarTodosReclamosResponse(listaDtos);
    }

}

public record ReclamoDTO(
    string Id, 
    string Asunto, 
    string Estado, 
    DateTime FechaCreacion,
    DateTime FechaUltimaModificacion
);
public record ListarTodosReclamosRequest();
public record ListarTodosReclamosResponse(IEnumerable<ReclamoDTO> lista);