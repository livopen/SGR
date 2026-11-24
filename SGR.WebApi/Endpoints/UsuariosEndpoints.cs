using System.Security.Claims;
using SGR.Aplicacion;
using SGR.Aplicacion.Usuarios;
using SGR.Dominio.Usuarios;

namespace SGR.WebApi.Endpoints;

public static class UsuariosEndpoints
{
    public static void MapUsuariosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/usuarios")
            .WithTags("Usuarios")
            .RequireAuthorization();

        // PUT /usuarios/yo (Modifica los datos propios extrayendo el ID del token JWT)
        group.MapPut("/yo", (ModificarMisDatosHttpBody body, ClaimsPrincipal user, ModificarDatosUsuarioUseCase useCase) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var request = new ModificarDatosUsuarioRequest(
                UserIdDesdeToken: userId,
                UserIdAModificar: userId,
                NuevoNombre: body.NuevoNombre,
                NuevoCorreo: body.NuevoCorreo,
                NuevacontraseniaPura: body.NuevaContrasenia ?? string.Empty
            );
            useCase.Ejecutar(request);
            return Results.NoContent();
        }).WithName("ModificarMisDatos")
          .WithSummary("Actualizar los datos de mi cuenta");

        // GET /usuarios (Listado exclusivo para Administrador)
        group.MapGet("/", (ClaimsPrincipal user, ListarUsuariosUseCase useCase) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = useCase.Ejecutar(new ListarUsuariosRequest(userId));
            return Results.Ok(response);
        }).WithName("ListarUsuarios")
          .WithSummary("Consultar los usuarios del sistema");

        // DELETE /usuarios/{id} (Eliminación exclusiva por Administrador)
        group.MapDelete("/{id:guid}", (Guid id, ClaimsPrincipal user, EliminarUsuarioUseCase useCase) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            useCase.Ejecutar(new EliminarUsuarioRequest(OperadorId: userId, UsuarioAEliminar: id));
            return Results.NoContent();
        }).WithName("EliminarUsuario")
          .WithSummary("Eliminar un usuario por su identificador");

        // PUT /usuarios/{id}/permisos (Modificación de permisos por Administrador)
        group.MapPut("/{id:guid}/permisos", (Guid id, ModificarPermisosHttpBody body, ClaimsPrincipal user, ModificarPermisosUsuarioUseCase useCase) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var request = new ModificarPermisosRequest(
                UsuarioId: id,
                NuevosPermisos: body.NuevosPermisos,
                IdUsuarioOperador: userId
            );
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).WithName("ModificarPermisosUsuario")
          .WithSummary("Actualizar los permisos de un usuario");
    }
}

public record ModificarMisDatosHttpBody(string NuevoNombre, string NuevoCorreo, string? NuevaContrasenia);
public record ModificarPermisosHttpBody(List<Permiso> NuevosPermisos);