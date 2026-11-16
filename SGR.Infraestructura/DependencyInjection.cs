using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SGR.Aplicacion; 
using SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Reclamos;
using SGR.Aplicacion.Usuarios;
using SGR.Aplicacion.Autorizacion;
using SGR.Aplicacion.Token;
using SGR.Infraestructura.Repository;

namespace SGR.Infraestructura;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraestructura(this IServiceCollection services, IConfiguration configuration)
    {
      
        var connectionString = configuration.GetConnectionString("SGEDb");
        services.AddDbContext<SgrContext>(opciones => opciones.UseSqlite(configuration.GetConnectionString("DefaultConnection") 
        ?? "Server=localhost\\SQLEXPRESS;Database=SGE_DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"));
        
        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
        services.AddScoped<IReclamoRepository, ReclamoRepository>();
        services.AddScoped<IActuacionRepository, ActuacionRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAutorizacionService, AutorizacionService>(); 
        
        return services;
    }
}