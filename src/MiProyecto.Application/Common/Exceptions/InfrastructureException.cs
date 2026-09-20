using MiProyecto.Application.Common.Errors;

namespace MiProyecto.Application.Common.Exceptions;

public sealed class InfrastructureException : AppException
{
    public InfrastructureException(
        string message,
        string code = ErrorCodes.InfraUnavailable,
        Exception? innerException = null)
        : base(message, code, innerException: innerException)
    {
    }
}
