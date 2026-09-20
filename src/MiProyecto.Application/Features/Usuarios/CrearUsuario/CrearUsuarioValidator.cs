using FluentValidation;

namespace MiProyecto.Application.Features.Usuarios.CrearUsuario;

public sealed class CrearUsuarioValidator : AbstractValidator<CrearUsuarioCommand>
{
    public CrearUsuarioValidator()
    {
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
