using MiProyecto.Application.Abstractions;
using MiProyecto.Application.Common.Errors;
using MiProyecto.Application.Common.Exceptions;
using MiProyecto.Application.Features.Usuarios;
using MiProyecto.Application.Features.Usuarios.ObtenerUsuario;
using NSubstitute;

namespace MiProyecto.UnitTests.Features.Usuarios;

public sealed class ObtenerUsuarioHandlerTests
{
    [Fact]
    public async Task Handle_CuandoExiste_DevuelveElUsuario()
    {
        var repositorio = Substitute.For<IUsuarioRepository>();
        var esperado = UsuarioTestData.Demo();
        repositorio.ObtenerAsync(10, Arg.Any<CancellationToken>()).Returns(esperado);

        var handler = new ObtenerUsuarioHandler(repositorio);
        var result = await handler.Handle(new ObtenerUsuarioQuery { Id = 10 }, CancellationToken.None);

        Assert.Equal(esperado.Id, result.Id);
        Assert.Equal(esperado.Correo, result.Correo);
    }

    [Fact]
    public async Task Handle_CuandoNoExiste_LanzaNotFound()
    {
        var repositorio = Substitute.For<IUsuarioRepository>();
        repositorio.ObtenerAsync(99, Arg.Any<CancellationToken>()).Returns((UsuarioDto?)null);

        var handler = new ObtenerUsuarioHandler(repositorio);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new ObtenerUsuarioQuery { Id = 99 }, CancellationToken.None));

        Assert.Equal(ErrorCodes.Usuarios.NotFound, exception.Code);
        Assert.Equal("El usuario no existe", exception.Message);
    }
}
