using Catalogo.Dominio.Excepciones;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Catalogo.Api.Middleware;

/// <summary>Traduce excepciones de negocio a respuestas HTTP (ProblemDetails) en un solo lugar.</summary>
public class ManejoErroresMiddleware(RequestDelegate siguiente, ILogger<ManejoErroresMiddleware> log)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await siguiente(ctx);
        }
        catch (ValidationException ex)
        {
            var errores = ex.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            await Responder(ctx, StatusCodes.Status400BadRequest, "Datos inválidos", errores: errores);
        }
        catch (ReglaDeNegocioException ex)
        {
            await Responder(ctx, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (NoEncontradoException ex)
        {
            await Responder(ctx, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error no controlado");
            await Responder(ctx, StatusCodes.Status500InternalServerError, "Error interno del servidor.");
        }
    }

    private static Task Responder(HttpContext ctx, int estado, string detalle, IDictionary<string, string[]>? errores = null)
    {
        ctx.Response.StatusCode = estado;
        ctx.Response.ContentType = "application/problem+json";
        var problema = new ValidationProblemDetails(errores ?? new Dictionary<string, string[]>())
        {
            Status = estado,
            Title = detalle
        };
        return ctx.Response.WriteAsJsonAsync(problema);
    }
}
