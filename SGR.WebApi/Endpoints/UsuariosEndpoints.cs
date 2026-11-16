using System.Security.Claims;
using SGR.Aplicacion.Usuarios;

namespace SGR.WebApi.Endpoints;

public static class UsuariosEndpoints
{
    public static void MapUsuariosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/usuarios")
            .WithTags("Usuarios")
            .RequireAuthorization();

        group.MapPut("/yo", (ClaimsPrincipal user, ModificarMisDatosRequest request, ModificarMisDatosUseCase useCase) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            useCase.Ejecutar(request, userId);
            return Results.NoContent();
        });

        group.MapGet("/", (ClaimsPrincipal user, ListarUsuariosUseCase useCase) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = useCase.Ejecutar(userId);
            return Results.Ok(response);
        });

        group.MapDelete("/{id:guid}", (Guid id, ClaimsPrincipal user, EliminarUsuarioUseCase useCase) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            useCase.Ejecutar(id, userId);
            return Results.NoContent();
        });

        group.MapPut("/{id:guid}/permisos", (Guid id, ModificarPermisosUsuarioRequest request, ClaimsPrincipal user, ModificarPermisosUsuarioUseCase useCase) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            useCase.Ejecutar(id, request, userId);
            return Results.NoContent();
        });
    }
}