using FluentValidation;

namespace MiProyecto.Application.Features.Usuarios.ListarUsuarios;

public sealed class ListarUsuariosValidator : AbstractValidator<ListarUsuariosQuery>
{
    public ListarUsuariosValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("La página debe ser mayor o igual a 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("El tamaño de página debe estar entre 1 y 100.");

        RuleFor(x => x.Search)
            .MaximumLength(150)
            .When(x => !string.IsNullOrWhiteSpace(x.Search))
            .WithMessage("La búsqueda no puede superar 150 caracteres.");
    }
}
