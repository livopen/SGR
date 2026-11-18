using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SGR.Aplicacion.Autorizacion;
using SGR.Aplicacion.Comun;
using SGR.Dominio.Comun;

namespace SGR.WebApi.Middlewares;

public class ManejadorDeExcepcionesGlobales : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        if (exception is DominioException exDom && exDom.Message.Contains("registrado", StringComparison.OrdinalIgnoreCase))
        {
            problemDetails.Title = "Conflicto de datos";
            problemDetails.Status = StatusCodes.Status409Conflict;
            problemDetails.Detail = exception.Message;
        }
        else if (exception is DominioException || exception is AplicacionException || exception is ArgumentException)
        {
            problemDetails.Title = "Regla de negocio violada";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = exception.Message;
        }
        else if (exception is CredencialesInvalidasException)
        {
            problemDetails.Title = "No autenticado";
            problemDetails.Status = StatusCodes.Status401Unauthorized;
            problemDetails.Detail = exception.Message;
        }
        else if (exception is AutorizacionException)
        {
            problemDetails.Title = "Acceso denegado";
            problemDetails.Status = StatusCodes.Status403Forbidden;
            problemDetails.Detail = exception.Message;
        }
        else if (exception is EntidadNoEncontradaException)
        {
            problemDetails.Title = "Recurso no encontrado";
            problemDetails.Status = StatusCodes.Status404NotFound;
            problemDetails.Detail = exception.Message;
        }
        else
        {
            problemDetails.Title = "Error interno del servidor";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Detail = "Ha ocurrido un error inesperado en el servidor.";
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}