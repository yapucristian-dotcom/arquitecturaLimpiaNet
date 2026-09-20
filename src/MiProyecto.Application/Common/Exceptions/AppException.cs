using MiProyecto.Application.Common.Responses;

namespace MiProyecto.Application.Common.Exceptions;

public class AppException : Exception
{
    public string Code { get; }

    public IReadOnlyList<ApiError> Errors { get; }

    public AppException(
        string message,
        string code,
        IEnumerable<ApiError>? errors = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
        Errors = errors?.ToArray() ?? [];
    }
}
