using System.Security.Claims;
using SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Reclamos;

namespace SGR.WebApi.Endpoints;

public static class ReclamosEndpoints
{
    public static void MapReclamosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/reclamos")
            .WithTags("Reclamos")
            .RequireAuthorization();

        // GET /reclamos?estado={estado}
        group.MapGet("/", (string? estado, ListarTodosReclamosUseCase useCase) =>
        {
            var response = useCase.Ejecutar(new ListarTodosReclamosRequest());
            var resultado = response.lista;
            if (!string.IsNullOrWhiteSpace(estado))
            {
                resultado = resultado.Where(r => r.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase));
            }
            return Results.Ok(resultado);
        });

        // GET /reclamos/{id}
        group.MapGet("/{id:guid}", (Guid id, ObtenerReclamoPorIdUseCase useCase) =>
        {
            var response = useCase.Ejecutar(new ObtenerReclamoPorIdRequest(id.ToString()));
            return Results.Ok(response);
        });

        // POST /reclamos
        group.MapPost("/", (AgregarReclamoHttpBody body, ClaimsPrincipal user, AgregarReclamoUseCase useCase) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var request = new AgregarReclamoRequest(asunto: body.Asunto, IdUsuario: userId);
            var response = useCase.Ejecutar(request);
            return Results.Created($"/reclamos/{response.IdReclamo}", response);
        });

        // PUT /reclamos/{id}/asunto
        group.MapPut("/{id:guid}/asunto", (Guid id, ModificarAsuntoHttpBody body, ClaimsPrincipal user, ModificarAsuntoReclamoUseCase useCase) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var request = new ModificarAsuntoReclamoRequest(descripcion: body.NuevoAsunto, UsuarioId: userId, ReclamoId: id.ToString());
            useCase.Ejecutar(request);
            return Results.NoContent();
        });

        // PUT /reclamos/{id}/estado
        group.MapPut("/{id:guid}/estado", (Guid id, CambiarEstadoHttpBody body, ClaimsPrincipal user, CambiarEstadoReclamoUseCase useCase) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var request = new CambiarEstadoReclamoRequest(IdUsuario: userId, reclamoId: id.ToString(), NuevoEstado: body.NuevoEstado);
            useCase.Ejecutar(request);
            return Results.NoContent();
        });

        // DELETE /reclamos/{id}
        group.MapDelete("/{id:guid}", (Guid id, ClaimsPrincipal user, EliminarReclamoUseCase useCase) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var request = new EliminarReclamoRequest(IdReclamo: id.ToString(), UsuarioId: userId);
            useCase.Ejecutar(request);
            return Results.NoContent();
        });

        // GET /reclamos/{id}/actuaciones
        group.MapGet("/{id:guid}/actuaciones", (Guid id, ObtenerActuacionesPorIdReclamoUseCase useCase) =>
        {
            var response = useCase.Ejecutar(new ObtenerActuacionesPorIdReclamoRequest(id.ToString()));
            return Results.Ok(response);
        });

        // POST /reclamos/{id}/actuaciones
        group.MapPost("/{id:guid}/actuaciones", (Guid id, AgregarActuacionHttpBody body, ClaimsPrincipal user, AgregarActuacionUseCase useCase) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var request = new AgregarActuacionRequest(idReclamo: id.ToString(), tipo: body.Tipo, detalle: body.Detalle, IdUsuario: userId);
            var response = useCase.Ejecutar(request);
            return Results.Created($"/actuaciones/{response.IdReclamo}", response);
        });
    }
}

public record AgregarReclamoHttpBody(string Asunto);
public record ModificarAsuntoHttpBody(string NuevoAsunto);
public record CambiarEstadoHttpBody(string NuevoEstado);
public record AgregarActuacionHttpBody(string Tipo, string Detalle);