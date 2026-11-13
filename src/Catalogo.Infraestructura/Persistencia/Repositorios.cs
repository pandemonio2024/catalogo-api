using Catalogo.Aplicacion.Abstracciones;
using Catalogo.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Infraestructura.Persistencia;

public class CategoriaRepositorio(CatalogoDbContext db) : ICategoriaRepositorio
{
    // AsNoTracking: consulta de solo lectura, EF no rastrea los cambios (más rápido).
    public Task<List<Categoria>> ListarAsync(CancellationToken ct)
        => db.Categorias.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync(ct);

    public Task<bool> ExisteAsync(int id, CancellationToken ct)
        => db.Categorias.AnyAsync(c => c.Id == id, ct);

    public async Task AgregarAsync(Categoria categoria, CancellationToken ct)
        => await db.Categorias.AddAsync(categoria, ct);
}

public class ProductoRepositorio(CatalogoDbContext db) : IProductoRepositorio
{
    public Task<List<Producto>> ListarAsync(int? categoriaId, CancellationToken ct)
    {
        var consulta = db.Productos.AsNoTracking().Include(p => p.Categoria).AsQueryable();
        if (categoriaId.HasValue)
            consulta = consulta.Where(p => p.CategoriaId == categoriaId.Value);
        return consulta.OrderBy(p => p.Id).ToListAsync(ct);
    }

    // Con tracking: se va a modificar o eliminar.
    public Task<Producto?> ObtenerAsync(int id, CancellationToken ct)
        => db.Productos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AgregarAsync(Producto producto, CancellationToken ct)
        => await db.Productos.AddAsync(producto, ct);

    public void Eliminar(Producto producto) => db.Productos.Remove(producto);
}
