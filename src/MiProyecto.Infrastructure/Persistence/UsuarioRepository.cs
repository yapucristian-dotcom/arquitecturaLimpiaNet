using MiProyecto.Application.Abstractions;
using MiProyecto.Application.Common.Exceptions;
using MiProyecto.Application.Common.Pagination;
using MiProyecto.Application.Features.Usuarios;
using MiProyecto.Application.Features.Usuarios.ActualizarUsuario;
using MiProyecto.Application.Features.Usuarios.CrearUsuario;
using MiProyecto.Application.Features.Usuarios.ListarUsuarios;
using MiProyecto.Infrastructure.PostgREST;

namespace MiProyecto.Infrastructure.Persistence;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly IPostgrestClient _client;

    public UsuarioRepository(IPostgrestClient client)
    {
        _client = client;
    }

    public Task<UsuarioDto?> ObtenerAsync(long id, CancellationToken cancellationToken)
    {
        return _client.RpcAsync<UsuarioDto>(UsuarioFunctions.Obtener, new { p_id = id }, cancellationToken);
    }

    public async Task<PagedResult<UsuarioDto>> ListarAsync(ListarUsuariosQuery query, CancellationToken cancellationToken)
    {
        var rows = await _client.RpcListAsync<UsuarioListRow>(
            UsuarioFunctions.Listar,
            new
            {
                p_page = query.Page,
                p_page_size = query.PageSize,
                p_search = query.Search
            },
            cancellationToken);

        var items = rows
            .Select(row => new UsuarioDto
            {
                Id = row.Id,
                Nombre = row.Nombre,
                Correo = row.Correo,
                Activo = row.Activo
            })
            .ToArray();

        return new PagedResult<UsuarioDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = rows.FirstOrDefault()?.TotalCount ?? 0
        };
    }

    public async Task<long> CrearAsync(CrearUsuarioCommand command, CancellationToken cancellationToken)
    {
        var id = await _client.RpcAsync<long>(
            UsuarioFunctions.Crear,
            new
            {
                p_nombre = command.Nombre,
                p_correo = command.Correo
            },
            cancellationToken);

        if (id <= 0)
        {
            throw new InfrastructureException("PostgREST no devolvió el identificador del usuario creado.");
        }

        return id;
    }

    public Task ActualizarAsync(ActualizarUsuarioCommand command, CancellationToken cancellationToken)
    {
        return _client.RpcExecuteAsync(
            UsuarioFunctions.Actualizar,
            new
            {
                p_id = command.Id,
                p_nombre = command.Nombre,
                p_correo = command.Correo,
                p_activo = command.Activo
            },
            cancellationToken);
    }

    public Task EliminarAsync(long id, CancellationToken cancellationToken)
    {
        return _client.RpcExecuteAsync(
            UsuarioFunctions.Eliminar,
            new { p_id = id },
            cancellationToken);
    }

    private sealed class UsuarioListRow
    {
        public long Id { get; init; }

        public string Nombre { get; init; } = string.Empty;

        public string Correo { get; init; } = string.Empty;

        public bool Activo { get; init; }

        public int TotalCount { get; init; }
    }
}
