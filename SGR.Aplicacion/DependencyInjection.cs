using Microsoft.Extensions.DependencyInjection;
using SGR.Aplicacion.Reclamos;
using SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Usuarios;
using SGR.Aplicacion.GestorDeContraseñas;
namespace SGR.Aplicacion;

public static class DependencyInjection
{
    public static IServiceCollection AddAplicacion(this IServiceCollection services)
    {
        // Se usa Transient porque se crean, ejecutan su acción y se destruyen.
        //Servicios de Actuaciones
        services.AddScoped<AgregarActuacionUseCase>();
        services.AddScoped<EliminarActuacionUseCase>();
        services.AddScoped<ModificarActuacionUseCase>();
        services.AddScoped<ObtenerActuacionesPorIdReclamoUseCase>();
        services.AddScoped<ObtenerActuacionPorIdUseCase>();
        services.AddScoped<ActualizacionEstadoReclamoService>();
        
        //Servicios de Reclamos
        services.AddScoped<AgregarReclamoUseCase>();
        services.AddScoped<CambiarEstadoReclamoUseCase>();
        services.AddScoped<EliminarReclamoUseCase>();
        services.AddScoped<ModificarAsuntoReclamoUseCase>();
        services.AddScoped<ListarTodosReclamosUseCase>();
        services.AddScoped<ObtenerReclamoPorIdUseCase>();
        services.AddScoped<ObtenerReclamoPorIdUseCase>();

        //Servicios de usuarios
        services.AddScoped<RegistrarUsuarioUseCase>();
        services.AddScoped<EliminarUsuarioUseCase>();
        services.AddScoped<ModificarPermisosUsuarioUseCase>();
        services.AddScoped<ModificarDatosUsuarioUseCase>();
        services.AddScoped<ListarUsuariosUseCase>();
        services.AddScoped<IPasswordHasher, CryptographyPasswordHasher>();
        services.AddScoped<LoginUseCase>();
        return services;
    }
}