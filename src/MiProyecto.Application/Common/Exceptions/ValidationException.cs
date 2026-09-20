using MiProyecto.Application.Common.Errors;
using MiProyecto.Application.Common.Responses;

namespace MiProyecto.Application.Common.Exceptions;

public sealed class ValidationException : AppException
{
    public ValidationException(IEnumerable<ApiError> errors)
        : base("Se encontraron errores de validación", ErrorCodes.ValidationError, errors)
    {
    }

    public ValidationException(string field, string message)
        : this([new ApiError { Field = field, Message = message }])
    {
    }
}
