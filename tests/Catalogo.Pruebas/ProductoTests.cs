using Catalogo.Dominio.Entidades;
using Catalogo.Dominio.Excepciones;

namespace Catalogo.Pruebas;

public class ProductoTests
{
    [Fact]
    public void Crear_ConDatosValidos_AsignaLosValores()
    {
        var p = Producto.Crear("  Mouse  ", 25.5m, 10, 1);

        Assert.Equal("Mouse", p.Nombre);   // recorta espacios
        Assert.Equal(25.5m, p.Precio);
        Assert.Equal(10, p.Stock);
        Assert.Equal(1, p.CategoriaId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Crear_SinNombre_LanzaReglaDeNegocio(string nombre)
        => Assert.Throws<ReglaDeNegocioException>(() => Producto.Crear(nombre, 10m, 1, 1));

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Crear_ConPrecioNoPositivo_LanzaReglaDeNegocio(double precio)
        => Assert.Throws<ReglaDeNegocioException>(() => Producto.Crear("Mouse", (decimal)precio, 1, 1));

    [Fact]
    public void Crear_ConStockNegativo_LanzaReglaDeNegocio()
        => Assert.Throws<ReglaDeNegocioException>(() => Producto.Crear("Mouse", 10m, -1, 1));

    [Fact]
    public void Modificar_ActualizaLosCampos()
    {
        var p = Producto.Crear("Mouse", 10m, 5, 1);

        p.Modificar("Teclado", 30m, 2, 3);

        Assert.Equal("Teclado", p.Nombre);
        Assert.Equal(30m, p.Precio);
        Assert.Equal(2, p.Stock);
        Assert.Equal(3, p.CategoriaId);
    }

    [Fact]
    public void Categoria_SinNombre_LanzaReglaDeNegocio()
        => Assert.Throws<ReglaDeNegocioException>(() => Categoria.Crear(" "));
}
