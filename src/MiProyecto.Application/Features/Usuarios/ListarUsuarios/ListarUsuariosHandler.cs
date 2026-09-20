using MediatR;
using MiProyecto.Application.Abstractions;
using MiProyecto.Application.Common.Pagination;
using MiProyecto.Application.Features.Usuarios;

namespace MiProyecto.Application.Features.Usuarios.ListarUsuarios;

public sealed class ListarUsuariosHandler : IRequestHandler<ListarUsuariosQuery, PagedResult<UsuarioDto>>
{
    private readonly IUsuarioRepository _usuarios;

    public ListarUsuariosHandler(IUsuarioRepository usuarios)
    {
        _usuarios = usuarios;
    }

    public Task<PagedResult<UsuarioDto>> Handle(ListarUsuariosQuery request, CancellationToken cancellationToken)
    {
        return _usuarios.ListarAsync(request, cancellationToken);
    }
}
