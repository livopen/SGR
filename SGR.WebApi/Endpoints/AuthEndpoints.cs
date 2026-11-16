using SGR.Aplicacion.Usuarios;

namespace SGR.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Autenticación");

        group.MapPost("/registro", (RegistrarUsuarioUseCase useCase, RegistrarUsuarioRequest request) =>
        {
            var response = useCase.Ejecutar(request);
            return Results.Created($"/usuarios/{response.Id}", response);
        });

        group.MapPost("/login", (LoginUseCase useCase, LoginRequest request) =>
        {
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        });
    }
}