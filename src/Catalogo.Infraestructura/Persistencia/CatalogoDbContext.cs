using Catalogo.Aplicacion.Abstracciones;
using Catalogo.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Infraestructura.Persistencia;

public class CatalogoDbContext(DbContextOptions<CatalogoDbContext> options)
    : DbContext(options), IUnidadDeTrabajo
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();

    public Task<int> GuardarCambiosAsync(CancellationToken ct) => SaveChangesAsync(ct);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            e.HasIndex(c => c.Nombre).IsUnique();
            e.HasData(
                new { Id = 1, Nombre = "Electrónica" },
                new { Id = 2, Nombre = "Hogar" },
                new { Id = 3, Nombre = "Oficina" });
        });

        modelBuilder.Entity<Producto>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
            e.Property(p => p.Precio).HasPrecision(18, 2);
            e.HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasData(
                new { Id = 1, Nombre = "Audífonos Bluetooth", Precio = 89.90m, Stock = 25, CategoriaId = 1 },
                new { Id = 2, Nombre = "Licuadora 600W", Precio = 129.00m, Stock = 10, CategoriaId = 2 },
                new { Id = 3, Nombre = "Teclado mecánico", Precio = 159.50m, Stock = 15, CategoriaId = 3 });
        });
    }
}
