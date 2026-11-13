using Catalogo.Dominio.Excepciones;

namespace Catalogo.Dominio.Entidades;

public class Producto
{
    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public decimal Precio { get; private set; }
    public int Stock { get; private set; }
    public int CategoriaId { get; private set; }
    public Categoria? Categoria { get; private set; }

    private Producto() { } // requerido por EF Core

    public static Producto Crear(string nombre, decimal precio, int stock, int categoriaId)
    {
        var producto = new Producto { CategoriaId = categoriaId };
        producto.Modificar(nombre, precio, stock, categoriaId);
        return producto;
    }

    // Las reglas viven en el dominio, no en el controller.
    public void Modificar(string nombre, decimal precio, int stock, int categoriaId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ReglaDeNegocioException("El nombre del producto es obligatorio.");
        if (precio <= 0)
            throw new ReglaDeNegocioException("El precio debe ser mayor que cero.");
        if (stock < 0)
            throw new ReglaDeNegocioException("El stock no puede ser negativo.");

        Nombre = nombre.Trim();
        Precio = precio;
        Stock = stock;
        CategoriaId = categoriaId;
    }
}
