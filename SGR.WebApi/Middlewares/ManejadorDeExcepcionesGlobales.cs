using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SGR.Dominio.Comun;
using SGR.Aplicacion.Comun;
using SGR.Aplicacion.Autorizacion;

namespace SGR.WebApi.Middlewares;

public class ManejadorDeExcepcionesGlobales : IExceptionHandler
{
    private readonly ILogger<ManejadorDeExcepcionesGlobales> _logger;

    public ManejadorDeExcepcionesGlobales(ILogger<ManejadorDeExcepcionesGlobales> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Excepción capturada por la Web API: {Message}", exception.Message);

        var (statusCode, title) = exception switch
        {
            DominioException => (StatusCodes.Status400BadRequest, "Error de validación del negocio"),
            AutorizacionException => (StatusCodes.Status403Forbidden, "Acceso denegado"),
            EntidadNoEncontradaException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
            Exception ex when ex.GetType().Name == "AutenticacionException" => (StatusCodes.Status401Unauthorized, "Credenciales inválidas"),
            Exception ex when ex.Message.Contains("registrado", StringComparison.OrdinalIgnoreCase) => (StatusCodes.Status409Conflict, "Conflicto con el recurso existente"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = statusCode == StatusCodes.Status500InternalServerError 
                ? "Ha ocurrido un error inesperado en el servidor." 
                : exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}