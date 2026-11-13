using Catalogo.Dominio.Excepciones;

namespace Catalogo.Dominio.Entidades;

public class Categoria
{
    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public ICollection<Producto> Productos { get; private set; } = new List<Producto>();

    private Categoria() { } // requerido por EF Core

    // Factory method: la única forma de crear una categoría válida.
    public static Categoria Crear(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ReglaDeNegocioException("El nombre de la categoría es obligatorio.");

        return new Categoria { Nombre = nombre.Trim() };
    }
}
