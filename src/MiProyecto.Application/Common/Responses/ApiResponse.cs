using MiProyecto.Application.Common.Errors;

namespace MiProyecto.Application.Common.Responses;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public string Code { get; init; } = ErrorCodes.Ok;

    public T? Data { get; init; }

    public IReadOnlyList<ApiError> Errors { get; init; } = [];

    public string? CorrelationId { get; init; }
}

public static class ApiResponse
{
    public static ApiResponse<T> Ok<T>(
        T data,
        string message = "Operación exitosa",
        string? correlationId = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Code = ErrorCodes.Ok,
            Data = data,
            Errors = [],
            CorrelationId = correlationId
        };
    }

    public static ApiResponse<object?> Ok(
        string message = "Operación exitosa",
        string? correlationId = null)
    {
        return Ok<object?>(null, message, correlationId);
    }

    public static ApiResponse<T> Fail<T>(
        string message,
        string code,
        IEnumerable<ApiError>? errors = null,
        string? correlationId = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Code = code,
            Data = default,
            Errors = errors?.ToArray() ?? [],
            CorrelationId = correlationId
        };
    }

    public static ApiResponse<object?> Fail(
        string message,
        string code,
        IEnumerable<ApiError>? errors = null,
        string? correlationId = null)
    {
        return Fail<object?>(message, code, errors, correlationId);
    }
}
