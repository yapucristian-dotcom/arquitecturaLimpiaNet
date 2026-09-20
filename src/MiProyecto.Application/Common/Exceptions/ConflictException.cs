using MiProyecto.Application.Common.Errors;

namespace MiProyecto.Application.Common.Exceptions;

public sealed class ConflictException : AppException
{
    public ConflictException(string message, string code = ErrorCodes.Conflict)
        : base(message, code)
    {
    }
}
