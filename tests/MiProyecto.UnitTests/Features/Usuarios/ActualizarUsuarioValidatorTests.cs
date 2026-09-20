using MiProyecto.Application.Features.Usuarios.ActualizarUsuario;

namespace MiProyecto.UnitTests.Features.Usuarios;

public sealed class ActualizarUsuarioValidatorTests
{
    private readonly ActualizarUsuarioValidator _validator = new();

    [Fact]
    public void Validate_CuandoLosDatosSonValidos_EsValido()
    {
        var result = _validator.Validate(new ActualizarUsuarioCommand
        {
            Id = 10,
            Nombre = "Ana Pérez",
            Correo = "ana@miproyecto.local",
            Activo = true
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_CuandoElIdEsCero_EsInvalido()
    {
        var result = _validator.Validate(new ActualizarUsuarioCommand
        {
            Id = 0,
            Nombre = "Ana Pérez",
            Correo = "ana@miproyecto.local"
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "El identificador debe ser mayor que cero.");
    }
}
