using FluentValidation;

namespace MiProyecto.Application.Features.Usuarios.ObtenerUsuario;

public sealed class ObtenerUsuarioValidator : AbstractValidator<ObtenerUsuarioQuery>
{
    public ObtenerUsuarioValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El identificador debe ser mayor que cero.");
    }
}
