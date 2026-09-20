using FluentValidation;

namespace MiProyecto.Application.Features.Usuarios.ActualizarUsuario;

public sealed class ActualizarUsuarioValidator : AbstractValidator<ActualizarUsuarioCommand>
{
    public ActualizarUsuarioValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El identificador debe ser mayor que cero.");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio.")
            .MaximumLength(150)
            .WithMessage("El nombre no puede superar 150 caracteres.");

        RuleFor(x => x.Correo)
            .NotEmpty()
            .WithMessage("El correo es obligatorio.")
            .EmailAddress()
            .WithMessage("El correo no es válido.")
            .MaximumLength(255)
            .WithMessage("El correo no puede superar 255 caracteres.");
    }
}
