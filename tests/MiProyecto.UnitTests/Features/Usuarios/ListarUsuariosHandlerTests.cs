using MiProyecto.Application.Abstractions;
using MiProyecto.Application.Common.Pagination;
using MiProyecto.Application.Features.Usuarios;
using MiProyecto.Application.Features.Usuarios.ListarUsuarios;
using NSubstitute;

namespace MiProyecto.UnitTests.Features.Usuarios;

public sealed class ListarUsuariosHandlerTests
{
    [Fact]
    public async Task Handle_CuandoHayResultados_DevuelveLaPagina()
    {
        var repositorio = Substitute.For<IUsuarioRepository>();
        var query = new ListarUsuariosQuery { Page = 1, PageSize = 20 };
        var pagina = new PagedResult<UsuarioDto>
        {
            Items = [UsuarioTestData.Demo()],
            Page = 1,
            PageSize = 20,
            TotalCount = 1
        };

        repositorio.ListarAsync(query, Arg.Any<CancellationToken>()).Returns(pagina);

        var handler = new ListarUsuariosHandler(repositorio);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
    }
}
