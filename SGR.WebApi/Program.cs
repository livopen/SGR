using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SGR.Aplicacion;
using SGR.Aplicacion.Token;
using SGR.Infraestructura;
using SGR.WebApi.Endpoints;
using SGR.WebApi.Middlewares;
using SGR.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Inyección de OpenAPI, ProblemDetails y Manejador Global de Excepciones
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManejadorDeExcepcionesGlobales>();

builder.Services.AddScoped<ITokenService, JwtTokenService>();

// 2. Registro Modular de Aplicación e Infraestructura
builder.Services.AddAplicacion();
builder.Services.AddInfraestructura(builder.Configuration);

// 3. Configuración de Autenticación JWT y Autorización
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// 4. Pipeline de Middlewares (Orden obligatorio de la cátedra: ExceptionHandler -> AuthN -> AuthZ)
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

// 5. Scalar e Interfaz de Documentación en Desarrollo
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// 6. Registro de Endpoints Descentralizados
app.MapAuthEndpoints();
app.MapUsuariosEndpoints();
app.MapReclamosEndpoints();
app.MapActuacionesEndpoints();

app.Run();