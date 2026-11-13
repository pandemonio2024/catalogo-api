using Catalogo.Aplicacion.Categorias;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalogo.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoriasController(ISender mediador) : ControllerBase
{
    public record CategoriaRequest(string Nombre);

    [HttpGet]
    public async Task<ActionResult<List<CategoriaDto>>> Listar(CancellationToken ct)
        => Ok(await mediador.Send(new ListarCategoriasQuery(), ct));

    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> Crear(CategoriaRequest r, CancellationToken ct)
    {
        var creada = await mediador.Send(new CrearCategoriaCommand(r.Nombre), ct);
        return Created($"/api/v1/categorias/{creada.Id}", creada);
    }
}
