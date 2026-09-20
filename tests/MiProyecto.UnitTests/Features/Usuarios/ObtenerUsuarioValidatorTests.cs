using MiProyecto.Application.Features.Usuarios.ObtenerUsuario;

namespace MiProyecto.UnitTests.Features.Usuarios;

public sealed class ObtenerUsuarioValidatorTests
{
    private readonly ObtenerUsuarioValidator _validator = new();

    [Fact]
    public void Validate_CuandoElIdEsValido_EsValido()
    {
        Assert.True(_validator.Validate(new ObtenerUsuarioQuery { Id = 1 }).IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_CuandoElIdNoEsPositivo_EsInvalido(long id)
    {
        var result = _validator.Validate(new ObtenerUsuarioQuery { Id = id });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "El identificador debe ser mayor que cero.");
    }
}
