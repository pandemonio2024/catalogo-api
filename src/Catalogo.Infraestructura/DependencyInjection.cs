using Catalogo.Aplicacion.Abstracciones;
using Catalogo.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalogo.Infraestructura;

public static class DependencyInjection
{
    public static IServiceCollection AgregarInfraestructura(this IServiceCollection services, IConfiguration config)
    {
        var cadena = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Falta la cadena de conexión 'Default'.");

        services.AddDbContext<CatalogoDbContext>(o => o.UseSqlServer(cadena));
        services.AddScoped<IUnidadDeTrabajo>(sp => sp.GetRequiredService<CatalogoDbContext>());
        services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
        services.AddScoped<IProductoRepositorio, ProductoRepositorio>();
        return services;
    }
}
