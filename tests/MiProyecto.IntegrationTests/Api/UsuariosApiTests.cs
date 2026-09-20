using System.Net;
using System.Net.Http.Json;
using MiProyecto.Application.Common.Errors;
using MiProyecto.Application.Common.Exceptions;
using MiProyecto.Application.Common.Pagination;
using MiProyecto.Application.Features.Usuarios;
using MiProyecto.Application.Features.Usuarios.CrearUsuario;
using MiProyecto.Application.Features.Usuarios.ListarUsuarios;
using MiProyecto.IntegrationTests.Support;
using NSubstitute;

namespace MiProyecto.IntegrationTests.Api;

public sealed class UsuariosApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public UsuariosApiTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Obtener_SinToken_Devuelve401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/usuarios/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Crear_ConRolUser_Devuelve403()
    {
        using var client = _factory.CreateAuthenticatedClient("User");

        var response = await client.PostAsJsonAsync("/api/usuarios", new
        {
            nombre = "Ana Pérez",
            correo = "ana@miproyecto.local"
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Crear_ConDatosInvalidos_Devuelve400()
    {
        using var client = _factory.CreateAuthenticatedClient("Admin");

        var response = await client.PostAsJsonAsync("/api/usuarios", new
        {
            nombre = "",
            correo = "correo-invalido"
        });

        var payload = await response.ReadApiResponseAsync<object>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(ErrorCodes.ValidationError, payload.Code);
        Assert.NotEmpty(payload.Errors);
    }

    [Fact]
    public async Task Crear_ConAdmin_Devuelve201()
    {
        _factory.Usuarios
            .CrearAsync(Arg.Any<CrearUsuarioCommand>(), Arg.Any<CancellationToken>())
            .Returns(42);

        using var client = _factory.CreateAuthenticatedClient("Admin");
        var response = await client.PostAsJsonAsync("/api/usuarios", new
        {
            nombre = "Ana Pérez",
            correo = "ana@miproyecto.local"
        });

        var payload = await response.ReadApiResponseAsync<CrearUsuarioResult>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.Equal(42, payload.Data?.Id);
    }

    [Fact]
    public async Task Obtener_CuandoNoExiste_Devuelve404()
    {
        _factory.Usuarios
            .ObtenerAsync(99, Arg.Any<CancellationToken>())
            .Returns((UsuarioDto?)null);

        using var client = _factory.CreateAuthenticatedClient("User");
        var response = await client.GetAsync("/api/usuarios/99");
        var payload = await response.ReadApiResponseAsync<object>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(ErrorCodes.Usuarios.NotFound, payload.Code);
    }

    [Fact]
    public async Task Obtener_CuandoExiste_Devuelve200YCorrelationId()
    {
        _factory.Usuarios
            .ObtenerAsync(10, Arg.Any<CancellationToken>())
            .Returns(new UsuarioDto
            {
                Id = 10,
                Nombre = "Usuario Demo",
                Correo = "demo@miproyecto.local",
                Activo = true
            });

        using var client = _factory.CreateAuthenticatedClient("User");
        client.DefaultRequestHeaders.Add("X-Correlation-ID", "corr-test-001");

        var response = await client.GetAsync("/api/usuarios/10");
        var payload = await response.ReadApiResponseAsync<UsuarioDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("corr-test-001", response.Headers.GetValues("X-Correlation-ID").Single());
        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.Equal("corr-test-001", payload.CorrelationId);
        Assert.Equal("demo@miproyecto.local", payload.Data?.Correo);
    }

    [Fact]
    public async Task Listar_CuandoLaPaginaEsInvalida_Devuelve400()
    {
        using var client = _factory.CreateAuthenticatedClient("User");

        var response = await client.GetAsync("/api/usuarios?page=0&pageSize=20");
        var payload = await response.ReadApiResponseAsync<object>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(ErrorCodes.ValidationError, payload.Code);
    }

    [Fact]
    public async Task Listar_CuandoEsValido_Devuelve200()
    {
        _factory.Usuarios
            .ListarAsync(Arg.Any<ListarUsuariosQuery>(), Arg.Any<CancellationToken>())
            .Returns(new PagedResult<UsuarioDto>
            {
                Items = [new UsuarioDto { Id = 1, Nombre = "Demo", Correo = "demo@miproyecto.local", Activo = true }],
                Page = 1,
                PageSize = 20,
                TotalCount = 1
            });

        using var client = _factory.CreateAuthenticatedClient("User");
        var response = await client.GetAsync("/api/usuarios?page=1&pageSize=20");
        var payload = await response.ReadApiResponseAsync<PagedResult<UsuarioDto>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.Single(payload.Data!.Items);
    }

    [Fact]
    public async Task Eliminar_CuandoNoExiste_Devuelve404()
    {
        _factory.Usuarios
            .When(repositorio => repositorio.EliminarAsync(77, Arg.Any<CancellationToken>()))
            .Do(_ => throw new NotFoundException("El usuario no existe", ErrorCodes.Usuarios.NotFound));

        using var client = _factory.CreateAuthenticatedClient("Admin");
        var response = await client.DeleteAsync("/api/usuarios/77");
        var payload = await response.ReadApiResponseAsync<object>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(ErrorCodes.Usuarios.NotFound, payload.Code);
    }
}
