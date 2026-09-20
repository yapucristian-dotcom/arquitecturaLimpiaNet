using MiProyecto.Application.Features.Usuarios.CrearUsuario;

namespace MiProyecto.UnitTests.Features.Usuarios;

public sealed class CrearUsuarioValidatorTests
{
    private readonly CrearUsuarioValidator _validator = new();

    [Fact]
    public void Validate_CuandoLosDatosSonValidos_EsValido()
    {
        var result = _validator.Validate(new CrearUsuarioCommand
        {
            Nombre = "Ana Pérez",
            Correo = "ana@miproyecto.local"
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_CuandoElNombreEstaVacio_EsInvalido()
    {
        var result = _validator.Validate(new CrearUsuarioCommand
        {
            Nombre = "",
            Correo = "ana@miproyecto.local"
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "El nombre es obligatorio.");
    }

    [Fact]
    public void Validate_CuandoElCorreoNoEsValido_EsInvalido()
    {
        var result = _validator.Validate(new CrearUsuarioCommand
        {
            Nombre = "Ana Pérez",
            Correo = "correo-invalido"
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "El correo no es válido.");
    }
}
