using System.Security.Claims;
using SGR.Aplicacion.Actuaciones;

namespace SGR.WebApi.Endpoints;

public static class ActuacionesEndpoints
{
    public static void MapActuacionesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/actuaciones")
            .WithTags("Actuaciones")
            .RequireAuthorization();

        // POST /actuaciones
        group.MapPost("/", (AgregarActuacionEnActuacionesHttpBody body, ClaimsPrincipal user, AgregarActuacionUseCase useCase) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var request = new AgregarActuacionRequest(
                idReclamo: body.IdReclamo,
                tipo: body.Tipo,
                detalle: body.Detalle,
                IdUsuario: userId);
            var response = useCase.Ejecutar(request);
            return Results.Created($"/actuaciones/{response.IdReclamo}", response);
        }).WithName("CrearActuacion")
          .WithSummary("Registrar una actuación en un reclamo");

        // GET /actuaciones/{id}
        group.MapGet("/{id:guid}", (Guid id, ClaimsPrincipal user, ObtenerActuacionPorIdUseCase useCase) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var response = useCase.Ejecutar(new ObtenerActuacionPorIdRequest(idActuacion: id.ToString(), idUsuario: userId));
            return Results.Ok(response);
        }).WithName("ObtenerActuacionPorId")
          .WithSummary("Consultar una actuación por su identificador");

        // GET /actuaciones/reclamo/{idReclamo}
        group.MapGet("/reclamo/{idReclamo:guid}", (Guid idReclamo, ObtenerActuacionesPorIdReclamoUseCase useCase) =>
        {
            var response = useCase.Ejecutar(
                new ObtenerActuacionesPorIdReclamoRequest(idReclamo.ToString()));
            return Results.Ok(response);
        }).WithName("ObtenerActuacionesPorReclamo")
          .WithSummary("Listar las actuaciones asociadas a un reclamo");

        // PUT /actuaciones/{id}
        group.MapPut("/{id:guid}", (Guid id, ModificarActuacionHttpBody body, ClaimsPrincipal user, ModificarActuacionUseCase useCase) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var request = new ModificarActuacionRequest(idActuacion: id.ToString(), detalle: body.Detalle, tipo: body.Tipo, idUsuario: userId);
            useCase.Ejecutar(request);
            return Results.NoContent();
        }).WithName("ModificarActuacion")
          .WithSummary("Modificar el tipo y el detalle de una actuación");

        // DELETE /actuaciones/{id}
        group.MapDelete("/{id:guid}", (Guid id, ClaimsPrincipal user, EliminarActuacionUseCase useCase) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var request = new EliminarActuacionRequest(IdActuacion: id.ToString(), UsuarioId: userId);
            useCase.Ejecutar(request);
            return Results.NoContent();
        }).WithName("EliminarActuacion")
          .WithSummary("Eliminar una actuación");
    }
}

public record AgregarActuacionEnActuacionesHttpBody(string IdReclamo, string Tipo, string Detalle);
public record ModificarActuacionHttpBody(string Tipo, string Detalle);