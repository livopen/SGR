using SGR.Aplicacion.Usuarios;

namespace SGR.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Autenticación");

        // DTO auxiliar para recibir desde el body de la petición
        group.MapPost("/registro", (RegistrarUsuarioHttpBody body, RegistrarUsuarioUseCase useCase) =>
        {
            var request = new RegistrarUsuarioRequest(body.Nombre, body.CorreoElectronico, body.Contrasenia, false);
            var response = useCase.Ejecutar(request);
            return Results.Created($"/usuarios/{response.Id}", response);
        });

        group.MapPost("/login", (LoginRequest request, LoginUseCase useCase) =>
        {
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        });
    }
}

public record RegistrarUsuarioHttpBody(string? Nombre, string CorreoElectronico, string Contrasenia);