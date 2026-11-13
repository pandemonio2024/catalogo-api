using Catalogo.Aplicacion.Abstracciones;
using Catalogo.Aplicacion.Productos;
using Catalogo.Dominio.Entidades;
using Catalogo.Dominio.Excepciones;
using FluentValidation.TestHelper;

namespace Catalogo.Pruebas;

// Repositorios falsos en memoria: se prueba el caso de uso sin base de datos
// (se sustituye la implementación por otra que respeta la misma interfaz).
internal class ProductoRepoFalso : IProductoRepositorio
{
    public List<Producto> Datos { get; } = [];
    public Task<List<Producto>> ListarAsync(int? categoriaId, CancellationToken ct)
        => Task.FromResult(Datos.Where(p => categoriaId == null || p.CategoriaId == categoriaId).ToList());
    public Task<Producto?> ObtenerAsync(int id, CancellationToken ct)
        => Task.FromResult(Datos.FirstOrDefault(p => p.Id == id));
    public Task AgregarAsync(Producto producto, CancellationToken ct) { Datos.Add(producto); return Task.CompletedTask; }
    public void Eliminar(Producto producto) => Datos.Remove(producto);
}

internal class CategoriaRepoFalso(params int[] idsExistentes) : ICategoriaRepositorio
{
    public Task<List<Categoria>> ListarAsync(CancellationToken ct) => Task.FromResult(new List<Categoria>());
    public Task<bool> ExisteAsync(int id, CancellationToken ct) => Task.FromResult(idsExistentes.Contains(id));
    public Task AgregarAsync(Categoria categoria, CancellationToken ct) => Task.CompletedTask;
}

internal class UowFalso : IUnidadDeTrabajo
{
    public int Guardados { get; private set; }
    public Task<int> GuardarCambiosAsync(CancellationToken ct) { Guardados++; return Task.FromResult(1); }
}

public class CasosDeUsoTests
{
    [Fact]
    public async Task CrearProducto_ConCategoriaExistente_GuardaYDevuelveDto()
    {
        var productos = new ProductoRepoFalso();
        var uow = new UowFalso();
        var handler = new CrearProductoHandler(productos, new CategoriaRepoFalso(1), uow);

        var dto = await handler.Handle(new CrearProductoCommand("Mouse", 20m, 5, 1), CancellationToken.None);

        Assert.Equal("Mouse", dto.Nombre);
        Assert.Single(productos.Datos);
        Assert.Equal(1, uow.Guardados);
    }

    [Fact]
    public async Task CrearProducto_ConCategoriaInexistente_LanzaNoEncontrado_YNoGuarda()
    {
        var productos = new ProductoRepoFalso();
        var uow = new UowFalso();
        var handler = new CrearProductoHandler(productos, new CategoriaRepoFalso(1), uow);

        await Assert.ThrowsAsync<NoEncontradoException>(() =>
            handler.Handle(new CrearProductoCommand("Mouse", 20m, 5, 99), CancellationToken.None));

        Assert.Empty(productos.Datos);
        Assert.Equal(0, uow.Guardados);
    }

    [Fact]
    public async Task ObtenerProducto_Inexistente_LanzaNoEncontrado()
    {
        var handler = new ObtenerProductoHandler(new ProductoRepoFalso());

        await Assert.ThrowsAsync<NoEncontradoException>(() =>
            handler.Handle(new ObtenerProductoQuery(123), CancellationToken.None));
    }

    [Fact]
    public async Task EliminarProducto_Existente_LoQuitaYGuarda()
    {
        var productos = new ProductoRepoFalso();
        productos.Datos.Add(Producto.Crear("Mouse", 20m, 5, 1));
        var uow = new UowFalso();
        var handler = new EliminarProductoHandler(productos, uow);

        // Id = 0 porque aún no pasó por la base; coincide con el primer elemento.
        await handler.Handle(new EliminarProductoCommand(0), CancellationToken.None);

        Assert.Empty(productos.Datos);
        Assert.Equal(1, uow.Guardados);
    }

    [Fact]
    public void ValidadorCrearProducto_PrecioCero_DaError()
    {
        var resultado = new CrearProductoValidator().TestValidate(new CrearProductoCommand("Mouse", 0m, 1, 1));
        resultado.ShouldHaveValidationErrorFor(x => x.Precio);
    }

    [Fact]
    public void ValidadorCrearProducto_DatosValidos_NoDaErrores()
    {
        var resultado = new CrearProductoValidator().TestValidate(new CrearProductoCommand("Mouse", 10m, 1, 1));
        resultado.ShouldNotHaveAnyValidationErrors();
    }
}
