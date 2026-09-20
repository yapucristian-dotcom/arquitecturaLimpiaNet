using System.Text.Json;
using MiProyecto.Application.Common.Errors;
using MiProyecto.Application.Common.Exceptions;

namespace MiProyecto.Infrastructure.PostgREST;

internal static class PostgrestErrorTranslator
{
    public static Exception ToApplicationException(PostgrestException exception)
    {
        var businessCode = TryGetBusinessCode(exception);
        var sqlState = exception.Code;
        var status = exception.HttpStatus;
        var message = string.IsNullOrWhiteSpace(exception.Message)
            ? "Ocurrió un error al consultar PostgREST."
            : exception.Message;

        if (status == 404 || sqlState is "PT404" || IsNotFound(businessCode))
        {
            return new NotFoundException(message, businessCode ?? ErrorCodes.NotFound);
        }

        if (status == 409 || sqlState is "PT409" || businessCode == ErrorCodes.Conflict)
        {
            return new ConflictException(message, businessCode ?? ErrorCodes.Conflict);
        }

        if (status == 401)
        {
            return new AppException(message, ErrorCodes.Unauthorized);
        }

        if (status == 403)
        {
            return new AppException(message, ErrorCodes.Forbidden);
        }

        if (status >= 500 || sqlState is "PT500" or "PGRST")
        {
            return new InfrastructureException(message, ErrorCodes.InfraUnavailable, exception);
        }

        return new AppException(message, businessCode ?? ErrorCodes.Internal);
    }

    private static bool IsNotFound(string? businessCode) =>
        businessCode is ErrorCodes.NotFound or ErrorCodes.Usuarios.NotFound;

    private static string? TryGetBusinessCode(PostgrestException exception)
    {
        if (string.IsNullOrWhiteSpace(exception.Details))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(exception.Details);
            if (document.RootElement.ValueKind == JsonValueKind.Object &&
                document.RootElement.TryGetProperty("code", out var codeProperty))
            {
                return codeProperty.GetString();
            }
        }
        catch (JsonException)
        {
            // DETAIL puede no ser JSON; se ignora y se usa el SQLSTATE.
        }

        return null;
    }
}
