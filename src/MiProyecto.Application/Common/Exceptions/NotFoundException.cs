using MiProyecto.Application.Common.Errors;

namespace MiProyecto.Application.Common.Exceptions;

public sealed class NotFoundException : AppException
{
    public NotFoundException(string message, string code = ErrorCodes.NotFound)
        : base(message, code)
    {
    }
}
