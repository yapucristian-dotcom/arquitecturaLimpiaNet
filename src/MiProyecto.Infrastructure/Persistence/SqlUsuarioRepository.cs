using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiProyecto.Application.Abstractions;
using MiProyecto.Application.Common.Exceptions;
using MiProyecto.Application.Common.Pagination;
using MiProyecto.Application.Features.Usuarios;
using MiProyecto.Application.Features.Usuarios.ActualizarUsuario;
using MiProyecto.Application.Features.Usuarios.CrearUsuario;
using MiProyecto.Application.Features.Usuarios.ListarUsuarios;
using MiProyecto.Infrastructure.Database;
using MiProyecto.Infrastructure.SqlServer;

namespace MiProyecto.Infrastructure.Persistence;

public sealed class SqlUsuarioRepository : IUsuarioRepository
{
    private readonly ISqlConnectionFactory _connections;
    private readonly DatabaseOptions _options;
    private readonly ILogger<SqlUsuarioRepository> _logger;

    public SqlUsuarioRepository(
        ISqlConnectionFactory connections,
        IOptions<DatabaseOptions> options,
        ILogger<SqlUsuarioRepository> logger)
    {
        _connections = connections;
        _options = options.Value;
        _logger = logger;
    }

    public Task<UsuarioDto?> ObtenerAsync(long id, CancellationToken cancellationToken)
    {
        return RunAsync(
            UsuarioFunctions.Obtener,
            parameters => AddBigInt(parameters, "@p_id", id),
            async (command, token) =>
            {
                await using var reader = await command.ExecuteReaderAsync(token);
                if (!await reader.ReadAsync(token))
                {
                    return null;
                }

                return MapUsuario(reader);
            },
            cancellationToken);
    }

    public Task<PagedResult<UsuarioDto>> ListarAsync(ListarUsuariosQuery query, CancellationToken cancellationToken)
    {
        return RunAsync(
            UsuarioFunctions.Listar,
            parameters =>
            {
                AddInt(parameters, "@p_page", query.Page);
                AddInt(parameters, "@p_page_size", query.PageSize);
                AddString(parameters, "@p_search", query.Search, 255);
            },
            async (command, token) =>
            {
                var items = new List<UsuarioDto>();
                var totalCount = 0;

                await using var reader = await command.ExecuteReaderAsync(token);
                while (await reader.ReadAsync(token))
                {
                    items.Add(MapUsuario(reader));
                    totalCount = reader.GetInt32(reader.GetOrdinal("total_count"));
                }

                return new PagedResult<UsuarioDto>
                {
                    Items = items,
                    Page = query.Page,
                    PageSize = query.PageSize,
                    TotalCount = totalCount
                };
            },
            cancellationToken);
    }

    public Task<long> CrearAsync(CrearUsuarioCommand command, CancellationToken cancellationToken)
    {
        return RunAsync(
            UsuarioFunctions.Crear,
            parameters =>
            {
                AddString(parameters, "@p_nombre", command.Nombre, 150);
                AddString(parameters, "@p_correo", command.Correo, 255);
            },
            async (sqlCommand, token) =>
            {
                var scalar = await sqlCommand.ExecuteScalarAsync(token);
                if (scalar is null || scalar is DBNull)
                {
                    throw new InfrastructureException("SQL Server no devolvió el identificador del usuario creado.");
                }

                long id;
                try
                {
                    id = Convert.ToInt64(scalar);
                }
                catch (Exception exception) when (exception is FormatException or InvalidCastException or OverflowException)
                {
                    throw new InfrastructureException(
                        "SQL Server no devolvió el identificador del usuario creado.",
                        innerException: exception);
                }

                if (id <= 0)
                {
                    throw new InfrastructureException("SQL Server no devolvió el identificador del usuario creado.");
                }

                return id;
            },
            cancellationToken);
    }

    public async Task ActualizarAsync(ActualizarUsuarioCommand command, CancellationToken cancellationToken)
    {
        await RunAsync(
            UsuarioFunctions.Actualizar,
            parameters =>
            {
                AddBigInt(parameters, "@p_id", command.Id);
                AddString(parameters, "@p_nombre", command.Nombre, 150);
                AddString(parameters, "@p_correo", command.Correo, 255);
                AddBit(parameters, "@p_activo", command.Activo);
            },
            (sqlCommand, token) => sqlCommand.ExecuteNonQueryAsync(token),
            cancellationToken);
    }

    public async Task EliminarAsync(long id, CancellationToken cancellationToken)
    {
        await RunAsync(
            UsuarioFunctions.Eliminar,
            parameters => AddBigInt(parameters, "@p_id", id),
            (command, token) => command.ExecuteNonQueryAsync(token),
            cancellationToken);
    }

    private async Task<T> RunAsync<T>(
        string procedure,
        Action<SqlParameterCollection> bind,
        Func<SqlCommand, CancellationToken, Task<T>> action,
        CancellationToken cancellationToken)
    {
        await using var connection = _connections.Create();

        try
        {
            await connection.OpenAsync(cancellationToken);
        }
        catch (SqlException exception)
        {
            _logger.LogError(exception, "No fue posible abrir la conexión SQL Server para {Procedure}", procedure);
            throw SqlServerExceptionTranslator.ToConnectionException(exception);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = "api." + procedure;
        command.CommandType = CommandType.StoredProcedure;
        command.CommandTimeout = _options.CommandTimeoutSeconds;
        bind(command.Parameters);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await action(command, cancellationToken);
            stopwatch.Stop();
            _logger.LogInformation(
                "SQL Server {Procedure} en {ElapsedMs} ms",
                procedure,
                stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (SqlException exception)
        {
            stopwatch.Stop();
            _logger.LogWarning(
                exception,
                "SQL Server {Procedure} falló con {ErrorNumber} en {ElapsedMs} ms",
                procedure,
                exception.Number,
                stopwatch.ElapsedMilliseconds);
            throw SqlServerExceptionTranslator.ToApplicationException(exception);
        }
    }

    private static UsuarioDto MapUsuario(SqlDataReader reader)
    {
        return new UsuarioDto
        {
            Id = reader.GetInt64(reader.GetOrdinal("id")),
            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
            Correo = reader.GetString(reader.GetOrdinal("correo")),
            Activo = reader.GetBoolean(reader.GetOrdinal("activo"))
        };
    }

    private static void AddBigInt(SqlParameterCollection parameters, string name, long value)
    {
        parameters.Add(new SqlParameter(name, SqlDbType.BigInt) { Value = value });
    }

    private static void AddInt(SqlParameterCollection parameters, string name, int value)
    {
        parameters.Add(new SqlParameter(name, SqlDbType.Int) { Value = value });
    }

    private static void AddBit(SqlParameterCollection parameters, string name, bool value)
    {
        parameters.Add(new SqlParameter(name, SqlDbType.Bit) { Value = value });
    }

    private static void AddString(SqlParameterCollection parameters, string name, string? value, int size)
    {
        parameters.Add(new SqlParameter(name, SqlDbType.NVarChar, size)
        {
            Value = string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim()
        });
    }
}
