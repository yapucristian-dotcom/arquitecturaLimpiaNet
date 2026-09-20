using MediatR;
using MiProyecto.Application.Abstractions;
using MiProyecto.Application.Features.Usuarios.EliminarUsuario;
using NSubstitute;

namespace MiProyecto.UnitTests.Features.Usuarios;

public sealed class EliminarUsuarioHandlerTests
{
    [Fact]
    public async Task Handle_CuandoElimina_LlamaAlRepositorio()
    {
        var repositorio = Substitute.For<IUsuarioRepository>();
        var handler = new EliminarUsuarioHandler(repositorio);

        var result = await handler.Handle(new EliminarUsuarioCommand { Id = 10 }, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        await repositorio.Received(1).EliminarAsync(10, Arg.Any<CancellationToken>());
    }
}
