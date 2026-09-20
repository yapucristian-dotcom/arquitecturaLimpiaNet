using Testcontainers.PostgreSql;

namespace MiProyecto.IntegrationTests.Database;

public sealed class PostgresFixture : IAsyncLifetime
{
    public PostgreSqlContainer? Container { get; private set; }

    public bool IsAvailable { get; private set; }

    public string? SkipReason { get; private set; }

    public async Task InitializeAsync()
    {
        try
        {
            Container = new PostgreSqlBuilder("postgres:16-alpine")
                .WithDatabase("miproyecto")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            await Container.StartAsync();
            IsAvailable = true;
        }
        catch (Exception exception)
        {
            IsAvailable = false;
            SkipReason = $"Docker no está disponible: {exception.Message}";
        }
    }

    public async Task DisposeAsync()
    {
        if (Container is not null)
        {
            await Container.DisposeAsync();
        }
    }
}

[CollectionDefinition("postgres")]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>
{
}
