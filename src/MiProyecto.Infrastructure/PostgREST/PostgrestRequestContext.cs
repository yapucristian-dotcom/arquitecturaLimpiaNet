namespace MiProyecto.Infrastructure.PostgREST;

public sealed class PostgrestRequestContext : IPostgrestRequestContext
{
    public string? CorrelationId { get; set; }

    public string? AccessToken { get; set; }
}
