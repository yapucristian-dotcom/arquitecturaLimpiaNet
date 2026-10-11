using Microsoft.Data.SqlClient;

namespace MiProyecto.Infrastructure.SqlServer;

public interface ISqlConnectionFactory
{
    SqlConnection Create();
}
