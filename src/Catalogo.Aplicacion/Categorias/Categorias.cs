using Catalogo.Aplicacion.Abstracciones;
using Catalogo.Dominio.Entidades;
using FluentValidation;
using MediatR;

namespace Catalogo.Aplicacion.Categorias;

public record CategoriaDto(int Id, string Nombre);

// ---- Query: listar ----
public record ListarCategoriasQuery : IRequest<List<CategoriaDto>>;

public class ListarCategoriasHandler(ICategoriaRepositorio repo)
    : IRequestHandler<ListarCategoriasQuery, List<CategoriaDto>>
{
    public async Task<List<CategoriaDto>> Handle(ListarCategoriasQuery q, CancellationToken ct)
        => (await repo.ListarAsync(ct)).Select(c => new CategoriaDto(c.Id, c.Nombre)).ToList();
}

// ---- Command: crear ----
public record CrearCategoriaCommand(string Nombre) : IRequest<CategoriaDto>;

public class CrearCategoriaValidator : AbstractValidator<CrearCategoriaCommand>
{
    public CrearCategoriaValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
    }
}

public class CrearCategoriaHandler(ICategoriaRepositorio repo, IUnidadDeTrabajo uow)
    : IRequestHandler<CrearCategoriaCommand, CategoriaDto>
{
    public async Task<CategoriaDto> Handle(CrearCategoriaCommand cmd, CancellationToken ct)
    {
        var categoria = Categoria.Crear(cmd.Nombre);
        await repo.AgregarAsync(categoria, ct);
        await uow.GuardarCambiosAsync(ct);
        return new CategoriaDto(categoria.Id, categoria.Nombre);
    }
}
