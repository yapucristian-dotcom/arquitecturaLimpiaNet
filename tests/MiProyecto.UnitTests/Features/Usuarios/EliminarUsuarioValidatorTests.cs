using MiProyecto.Application.Features.Usuarios.EliminarUsuario;

namespace MiProyecto.UnitTests.Features.Usuarios;

public sealed class EliminarUsuarioValidatorTests
{
    private readonly EliminarUsuarioValidator _validator = new();

    [Fact]
    public void Validate_CuandoElIdEsValido_EsValido()
    {
        Assert.True(_validator.Validate(new EliminarUsuarioCommand { Id = 8 }).IsValid);
    }

    [Fact]
    public void Validate_CuandoElIdEsCero_EsInvalido()
    {
        var result = _validator.Validate(new EliminarUsuarioCommand { Id = 0 });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "El identificador debe ser mayor que cero.");
    }
}
