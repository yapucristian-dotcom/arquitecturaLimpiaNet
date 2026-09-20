namespace MiProyecto.Infrastructure.PostgREST;

public sealed class PostgrestException : Exception
{
    public string? Code { get; }

    public int HttpStatus { get; }

    public string? Details { get; }

    public string? Hint { get; }

    public PostgrestException(
        string message,
        int httpStatus,
        string? code = null,
        string? details = null,
        string? hint = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        HttpStatus = httpStatus;
        Code = code;
        Details = details;
        Hint = hint;
    }
}
