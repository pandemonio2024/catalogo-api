using Catalogo.Aplicacion.Comportamientos;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Catalogo.Aplicacion;

public static class DependencyInjection
{
    public static IServiceCollection AgregarAplicacion(this IServiceCollection services)
    {
        var ensamblado = typeof(DependencyInjection).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ensamblado));
        services.AddValidatorsFromAssembly(ensamblado);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidacionBehavior<,>));
        return services;
    }
}
