using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MiProyecto.Infrastructure.Database;

namespace MiProyecto.Infrastructure.SqlServer;

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IOptions<DatabaseOptions> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public SqlConnection Create() => new(_connectionString);
}
