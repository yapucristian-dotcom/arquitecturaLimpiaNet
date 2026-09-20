using FluentValidation;

namespace MiProyecto.Application.Features.Usuarios.EliminarUsuario;

public sealed class EliminarUsuarioValidator : AbstractValidator<EliminarUsuarioCommand>
{
    public EliminarUsuarioValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El identificador debe ser mayor que cero.");
    }
}
