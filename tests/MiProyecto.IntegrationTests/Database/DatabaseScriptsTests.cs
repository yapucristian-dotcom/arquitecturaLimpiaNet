using Npgsql;

namespace MiProyecto.IntegrationTests.Database;

[Collection("postgres")]
public sealed class DatabaseScriptsTests
{
    private readonly PostgresFixture _fixture;

    public DatabaseScriptsTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [SkippableFact]
    public async Task Scripts_InstalanYLasFunctionsResponden()
    {
        Skip.IfNot(_fixture.IsAvailable, _fixture.SkipReason ?? "Docker no está disponible.");

        var connectionString = _fixture.Container!.GetConnectionString();
        await DatabaseScriptRunner.ApplyAsync(connectionString);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var crear = new NpgsqlCommand(
            "SELECT api.fn_usuario_crear('Usuario Test', 'test@miproyecto.local');",
            connection);
        var id = (long)(await crear.ExecuteScalarAsync())!;

        await using var obtener = new NpgsqlCommand(
            "SELECT nombre, correo FROM api.fn_usuario_obtener(@id);",
            connection);
        obtener.Parameters.AddWithValue("id", id);

        await using var reader = await obtener.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("Usuario Test", reader.GetString(0));
        Assert.Equal("test@miproyecto.local", reader.GetString(1));
    }
}
