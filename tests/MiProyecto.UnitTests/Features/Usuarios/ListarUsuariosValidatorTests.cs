using MiProyecto.Application.Features.Usuarios.ListarUsuarios;

namespace MiProyecto.UnitTests.Features.Usuarios;

public sealed class ListarUsuariosValidatorTests
{
    private readonly ListarUsuariosValidator _validator = new();

    [Fact]
    public void Validate_CuandoLaPaginacionEsValida_EsValido()
    {
        var result = _validator.Validate(new ListarUsuariosQuery
        {
            Page = 1,
            PageSize = 20,
            Search = "ana"
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_CuandoLaPaginaEsCero_EsInvalido()
    {
        var result = _validator.Validate(new ListarUsuariosQuery { Page = 0, PageSize = 20 });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "La página debe ser mayor o igual a 1.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_CuandoElTamanoDePaginaEstaFueraDeRango_EsInvalido(int pageSize)
    {
        var result = _validator.Validate(new ListarUsuariosQuery { Page = 1, PageSize = pageSize });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "El tamaño de página debe estar entre 1 y 100.");
    }
}
