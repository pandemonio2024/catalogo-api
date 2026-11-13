using Catalogo.Aplicacion.Productos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalogo.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductosController(ISender mediador) : ControllerBase
{
    public record ProductoRequest(string Nombre, decimal Precio, int Stock, int CategoriaId);

    [HttpGet]
    public async Task<ActionResult<List<ProductoDto>>> Listar([FromQuery] int? categoriaId, CancellationToken ct)
        => Ok(await mediador.Send(new ListarProductosQuery(categoriaId), ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductoDto>> Obtener(int id, CancellationToken ct)
        => Ok(await mediador.Send(new ObtenerProductoQuery(id), ct));

    [HttpPost]
    public async Task<ActionResult<ProductoDto>> Crear(ProductoRequest r, CancellationToken ct)
    {
        var creado = await mediador.Send(new CrearProductoCommand(r.Nombre, r.Precio, r.Stock, r.CategoriaId), ct);
        return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductoDto>> Actualizar(int id, ProductoRequest r, CancellationToken ct)
        => Ok(await mediador.Send(new ActualizarProductoCommand(id, r.Nombre, r.Precio, r.Stock, r.CategoriaId), ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        await mediador.Send(new EliminarProductoCommand(id), ct);
        return NoContent();
    }
}
