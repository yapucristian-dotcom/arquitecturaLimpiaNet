using MediatR;
using MiProyecto.Application.Abstractions;
using MiProyecto.Application.Features.Usuarios.ActualizarUsuario;
using NSubstitute;

namespace MiProyecto.UnitTests.Features.Usuarios;

public sealed class ActualizarUsuarioHandlerTests
{
    [Fact]
    public async Task Handle_CuandoActualiza_LlamaAlRepositorio()
    {
        var repositorio = Substitute.For<IUsuarioRepository>();
        var command = new ActualizarUsuarioCommand
        {
            Id = 10,
            Nombre = "Ana Pérez",
            Correo = "ana@miproyecto.local",
            Activo = true
        };

        var handler = new ActualizarUsuarioHandler(repositorio);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        await repositorio.Received(1).ActualizarAsync(command, Arg.Any<CancellationToken>());
    }
}
