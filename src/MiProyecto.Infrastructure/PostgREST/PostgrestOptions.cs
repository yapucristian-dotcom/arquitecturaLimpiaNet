namespace MiProyecto.Infrastructure.PostgREST;

public sealed class PostgrestOptions
{
    public const string SectionName = "PostgREST";

    public string BaseUrl { get; set; } = "http://localhost:3000";

    public int TimeoutSeconds { get; set; } = 30;

    public string? ApiKey { get; set; }

    public string ApiKeyHeader { get; set; } = "X-API-Key";

    public string? Jwt { get; set; }
}
