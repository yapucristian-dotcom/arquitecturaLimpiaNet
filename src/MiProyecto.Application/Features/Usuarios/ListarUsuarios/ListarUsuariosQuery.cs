using MediatR;
using MiProyecto.Application.Common.Pagination;
using MiProyecto.Application.Features.Usuarios;

namespace MiProyecto.Application.Features.Usuarios.ListarUsuarios;

public sealed class ListarUsuariosQuery : PagedRequest, IRequest<PagedResult<UsuarioDto>>
{
}
