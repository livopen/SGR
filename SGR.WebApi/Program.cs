using SGR.Infraestructura;
using SGR.Aplicacion;
using Scalar.AspNetCore;
using SGR.WebApi.Endpoints;
using SGR.WebApi;
using SGR.WebApi.Middlewares;
using SGR.Aplicacion.Token;
using SGR.Aplicacion.Usuarios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SGR.Dominio.Usuarios;
using SGR.Aplicacion.GestorDeContraseñas;
Console.WriteLine("Iniciando el sistema...");

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios básicos
builder.Services.AddControllers();
builder.Services.AddOpenApi();
//para poder ver los enums en los json de respuesta
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    {
        options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
// 2. Capas de la aplicación e infraestructura
builder.Services.AddAplicacion();
builder.Services.AddInfraestructura(builder.Configuration);

// 3. Registrar el proveedor de Tokens JWT
builder.Services.AddScoped<ITokenService, JwtTokenProvider>();

// 4. Soporte para el formato estándar de errores (RFC 7807) y Excepciones
builder.Services.AddProblemDetails(); 
builder.Services.AddExceptionHandler<ManejadorDeExcepcionesGlobales>(); 

// 5. Configuración del servicio de Autenticación por JWT 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, 
            ValidateAudience = true, 
            ValidateLifetime = true, 
            ValidateIssuerSigningKey = true, 
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };

        // Eventos de diagnóstico para loguear en consola
        opciones.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($" ERROR JWT: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($" DESAFÍO JWT (401 enviado): {context.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// 1. Agregar política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.AllowAnyOrigin()   
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Authorization", "token", "Authorization-Token");
    });
});
var app = builder.Build();

// BLOQUE DE INICIALIZACIÓN: Creación de BD y Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<SgrContext>();
    var usuarioRepository = services.GetRequiredService<IUsuarioRepository>();
    var passwordHasher = services.GetRequiredService<IPasswordHasher>();

    context.Database.EnsureCreated();

    var adminCorreo = new CorreoElectronico("admin@sgr.com");
    if (usuarioRepository.ObtenerPorCorreo(adminCorreo) == null)
    {
        var nuevoAdmin = Usuario.UsuarioFactoryMethod(
            id: Guid.Parse("00000000-0000-0000-0000-000000000001"),
            nombre: "Admin inicial",
            correo: adminCorreo,
            contraseniaHash: passwordHasher.HashPassword("admin123"),
            esAdministrador: true
        );
        context.Usuarios.Add(nuevoAdmin);
    }

    var op1Correo = new CorreoElectronico("operador1@sgr.com");
    if (usuarioRepository.ObtenerPorCorreo(op1Correo) == null)
    {
        var nuevoOp1 = Usuario.UsuarioFactoryMethod(
            id: Guid.Parse("00000000-0000-0000-0000-000000000002"),
            nombre: "Operador Uno",
            correo: op1Correo,
            contraseniaHash: passwordHasher.HashPassword("123operador"),
            esAdministrador: false
        );
        context.Usuarios.Add(nuevoOp1);
    }

    var op2Correo = new CorreoElectronico("operador2@sgr.com");
    if (usuarioRepository.ObtenerPorCorreo(op2Correo) == null)
    {
        var nuevoOp2 = Usuario.UsuarioFactoryMethod(
            id: Guid.Parse("00000000-0000-0000-0000-000000000003"),
            nombre: "Operador Dos",
            correo: op2Correo,
            contraseniaHash: passwordHasher.HashPassword("123operador"),
            esAdministrador: false
        );
        context.Usuarios.Add(nuevoOp2);
    }

    context.SaveChanges();
}
Console.WriteLine("¡Base de datos y tablas verificadas/creadas con éxito!");

// PIPELINE HTTP
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("SGR API - Sistema de Gestión de Reclamos")
            .WithTheme(ScalarTheme.Purple)
            .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
    });
}

app.UseCors("AllowBlazor");
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapGet("/", () => "¡La API está funcionando!");
app.MapUsuariosEndpoints();
app.MapAuthEndpoints();
app.MapReclamosEndpoints();
app.MapActuacionesEndpoints();
app.MapControllers();

app.Run();

public record ModificarPermisosBody(List<SGR.Dominio.Usuarios.Permiso> NuevosPermisos);
public record ModificarMisDatosBody(string NuevoNombre, string NuevoCorreo, string NuevacontraseniaPura);