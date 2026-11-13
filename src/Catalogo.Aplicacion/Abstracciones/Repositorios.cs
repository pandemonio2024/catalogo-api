using Catalogo.Dominio.Entidades;

namespace Catalogo.Aplicacion.Abstracciones;

// Las interfaces viven en Aplicación; la implementación (EF Core) en Infraestructura.
// Así la dependencia apunta hacia adentro (inversión de dependencias).
public interface ICategoriaRepositorio
{
    Task<List<Categoria>> ListarAsync(CancellationToken ct);
    Task<bool> ExisteAsync(int id, CancellationToken ct);
    Task AgregarAsync(Categoria categoria, CancellationToken ct);
}

public interface IProductoRepositorio
{
    Task<List<Producto>> ListarAsync(int? categoriaId, CancellationToken ct);
    Task<Producto?> ObtenerAsync(int id, CancellationToken ct);
    Task AgregarAsync(Producto producto, CancellationToken ct);
    void Eliminar(Producto producto);
}

public interface IUnidadDeTrabajo
{
    Task<int> GuardarCambiosAsync(CancellationToken ct);
}
