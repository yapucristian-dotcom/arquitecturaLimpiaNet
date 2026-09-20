using MiProyecto.Application.Common.Pagination;
using MiProyecto.Application.Features.Usuarios;
using MiProyecto.Application.Features.Usuarios.ActualizarUsuario;
using MiProyecto.Application.Features.Usuarios.CrearUsuario;
using MiProyecto.Application.Features.Usuarios.ListarUsuarios;

namespace MiProyecto.Application.Abstractions;

public interface IUsuarioRepository
{
    Task<UsuarioDto?> ObtenerAsync(long id, CancellationToken cancellationToken);

    Task<PagedResult<UsuarioDto>> ListarAsync(ListarUsuariosQuery query, CancellationToken cancellationToken);

    Task<long> CrearAsync(CrearUsuarioCommand command, CancellationToken cancellationToken);

    Task ActualizarAsync(ActualizarUsuarioCommand command, CancellationToken cancellationToken);

    Task EliminarAsync(long id, CancellationToken cancellationToken);
}
