using MiProyecto.Application.Abstractions;
using MiProyecto.Application.Features.Usuarios.CrearUsuario;
using NSubstitute;

namespace MiProyecto.UnitTests.Features.Usuarios;

public sealed class CrearUsuarioHandlerTests
{
    [Fact]
    public async Task Handle_CuandoElRepositorioCrea_DevuelveElId()
    {
        var repositorio = Substitute.For<IUsuarioRepository>();
        var command = new CrearUsuarioCommand
        {
            Nombre = "Ana Pérez",
            Correo = "ana@miproyecto.local"
        };

        repositorio.CrearAsync(command, Arg.Any<CancellationToken>()).Returns(25);

        var handler = new CrearUsuarioHandler(repositorio);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(25, result.Id);
        await repositorio.Received(1).CrearAsync(command, Arg.Any<CancellationToken>());
    }
}
