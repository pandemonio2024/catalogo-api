using FluentValidation;
using MediatR;

namespace Catalogo.Aplicacion.Comportamientos;

/// <summary>
/// Pipeline de MediatR: valida cada comando/consulta con FluentValidation
/// antes de llegar al handler (patrón Decorator / Chain of Responsibility).
/// </summary>
public class ValidacionBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validadores)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (validadores.Any())
        {
            var contexto = new ValidationContext<TRequest>(request);
            var resultados = await Task.WhenAll(validadores.Select(v => v.ValidateAsync(contexto, ct)));
            var errores = resultados.SelectMany(r => r.Errors).Where(e => e != null).ToList();
            if (errores.Count != 0)
                throw new ValidationException(errores);
        }
        return await next();
    }
}
