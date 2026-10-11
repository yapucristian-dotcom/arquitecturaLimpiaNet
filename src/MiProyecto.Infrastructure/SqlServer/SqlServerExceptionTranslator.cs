using Microsoft.Data.SqlClient;
using MiProyecto.Application.Common.Errors;
using MiProyecto.Application.Common.Exceptions;

namespace MiProyecto.Infrastructure.SqlServer;

internal static class SqlServerExceptionTranslator
{
    public const int NotFound = 50404;

    public const int Conflict = 50409;

    public static Exception ToConnectionException(SqlException exception) =>
        new InfrastructureException(
            "No fue posible comunicarse con SQL Server.",
            ErrorCodes.InfraUnavailable,
            exception);

    public static Exception ToApplicationException(SqlException exception)
    {
        if (exception.Number == NotFound)
        {
            return new NotFoundException(
                string.IsNullOrWhiteSpace(exception.Message) ? "El usuario no existe" : exception.Message,
                ErrorCodes.Usuarios.NotFound);
        }

        if (exception.Number == Conflict || exception.Number is 2601 or 2627)
        {
            return new ConflictException(
                exception.Number is 2601 or 2627 || string.IsNullOrWhiteSpace(exception.Message)
                    ? "El correo ya está registrado"
                    : exception.Message,
                ErrorCodes.Conflict);
        }

        return new InfrastructureException(
            "No fue posible comunicarse con SQL Server.",
            ErrorCodes.InfraUnavailable,
            exception);
    }
}
