namespace MiProyecto.Infrastructure.PostgREST;

public interface IPostgrestRequestContext
{
    string? CorrelationId { get; set; }

    string? AccessToken { get; set; }
}
