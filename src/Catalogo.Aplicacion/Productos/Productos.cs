using Catalogo.Aplicacion.Abstracciones;
using Catalogo.Dominio.Entidades;
using Catalogo.Dominio.Excepciones;
using FluentValidation;
using MediatR;

namespace Catalogo.Aplicacion.Productos;

public record ProductoDto(int Id, string Nombre, decimal Precio, int Stock, int CategoriaId, string? Categoria);

internal static class ProductoMapeo
{
    public static ProductoDto ADto(this Producto p)
        => new(p.Id, p.Nombre, p.Precio, p.Stock, p.CategoriaId, p.Categoria?.Nombre);
}

// ---------- Queries ----------
public record ListarProductosQuery(int? CategoriaId) : IRequest<List<ProductoDto>>;

public class ListarProductosHandler(IProductoRepositorio repo)
    : IRequestHandler<ListarProductosQuery, List<ProductoDto>>
{
    public async Task<List<ProductoDto>> Handle(ListarProductosQuery q, CancellationToken ct)
        => (await repo.ListarAsync(q.CategoriaId, ct)).Select(p => p.ADto()).ToList();
}

public record ObtenerProductoQuery(int Id) : IRequest<ProductoDto>;

public class ObtenerProductoHandler(IProductoRepositorio repo)
    : IRequestHandler<ObtenerProductoQuery, ProductoDto>
{
    public async Task<ProductoDto> Handle(ObtenerProductoQuery q, CancellationToken ct)
    {
        var producto = await repo.ObtenerAsync(q.Id, ct) ?? throw new NoEncontradoException("Producto", q.Id);
        return producto.ADto();
    }
}

// ---------- Commands ----------
public record CrearProductoCommand(string Nombre, decimal Precio, int Stock, int CategoriaId) : IRequest<ProductoDto>;

public class CrearProductoValidator : AbstractValidator<CrearProductoCommand>
{
    public CrearProductoValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Precio).GreaterThan(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoriaId).GreaterThan(0);
    }
}

public class CrearProductoHandler(IProductoRepositorio productos, ICategoriaRepositorio categorias, IUnidadDeTrabajo uow)
    : IRequestHandler<CrearProductoCommand, ProductoDto>
{
    public async Task<ProductoDto> Handle(CrearProductoCommand cmd, CancellationToken ct)
    {
        if (!await categorias.ExisteAsync(cmd.CategoriaId, ct))
            throw new NoEncontradoException("Categoría", cmd.CategoriaId);

        var producto = Producto.Crear(cmd.Nombre, cmd.Precio, cmd.Stock, cmd.CategoriaId);
        await productos.AgregarAsync(producto, ct);
        await uow.GuardarCambiosAsync(ct);
        return producto.ADto();
    }
}

public record ActualizarProductoCommand(int Id, string Nombre, decimal Precio, int Stock, int CategoriaId) : IRequest<ProductoDto>;

public class ActualizarProductoValidator : AbstractValidator<ActualizarProductoCommand>
{
    public ActualizarProductoValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Precio).GreaterThan(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoriaId).GreaterThan(0);
    }
}

public class ActualizarProductoHandler(IProductoRepositorio productos, ICategoriaRepositorio categorias, IUnidadDeTrabajo uow)
    : IRequestHandler<ActualizarProductoCommand, ProductoDto>
{
    public async Task<ProductoDto> Handle(ActualizarProductoCommand cmd, CancellationToken ct)
    {
        var producto = await productos.ObtenerAsync(cmd.Id, ct) ?? throw new NoEncontradoException("Producto", cmd.Id);
        if (!await categorias.ExisteAsync(cmd.CategoriaId, ct))
            throw new NoEncontradoException("Categoría", cmd.CategoriaId);

        producto.Modificar(cmd.Nombre, cmd.Precio, cmd.Stock, cmd.CategoriaId);
        await uow.GuardarCambiosAsync(ct);
        return producto.ADto();
    }
}

public record EliminarProductoCommand(int Id) : IRequest;

public class EliminarProductoHandler(IProductoRepositorio productos, IUnidadDeTrabajo uow)
    : IRequestHandler<EliminarProductoCommand>
{
    public async Task Handle(EliminarProductoCommand cmd, CancellationToken ct)
    {
        var producto = await productos.ObtenerAsync(cmd.Id, ct) ?? throw new NoEncontradoException("Producto", cmd.Id);
        productos.Eliminar(producto);
        await uow.GuardarCambiosAsync(ct);
    }
}
