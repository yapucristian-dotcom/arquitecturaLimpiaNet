using Npgsql;

namespace MiProyecto.IntegrationTests.Database;

internal static class DatabaseScriptRunner
{
    private static readonly string[] ScriptRelativePaths =
    [
        Path.Combine("schemas", "seguridad.sql"),
        Path.Combine("schemas", "negocio.sql"),
        Path.Combine("schemas", "api.sql"),
        Path.Combine("schemas", "reportes.sql"),
        Path.Combine("tables", "usuarios.sql"),
        Path.Combine("functions", "fn_usuario_obtener.sql"),
        Path.Combine("functions", "fn_usuario_listar.sql"),
        Path.Combine("functions", "fn_usuario_crear.sql"),
        Path.Combine("functions", "fn_usuario_actualizar.sql"),
        Path.Combine("functions", "fn_usuario_eliminar.sql"),
        Path.Combine("functions", "fn_ejemplo_transaccion.sql"),
        Path.Combine("schemas", "grants.sql"),
        Path.Combine("seed", "usuarios.sql")
    ];

    public static async Task ApplyAsync(string connectionString)
    {
        var databaseDirectory = Path.Combine(AppContext.BaseDirectory, "database");

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        foreach (var relativePath in ScriptRelativePaths)
        {
            var path = Path.Combine(databaseDirectory, relativePath);
            var sql = await File.ReadAllTextAsync(path);
            await using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}
