using System.Text.Json.Serialization;

namespace MiProyecto.Infrastructure.PostgREST;

internal sealed class PostgrestErrorResponse
{
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }

    [JsonPropertyName("details")]
    public string? Details { get; init; }

    [JsonPropertyName("hint")]
    public string? Hint { get; init; }
}
