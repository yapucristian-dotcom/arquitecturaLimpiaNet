namespace MiProyecto.Infrastructure.PostgREST;

public interface IPostgrestClient
{
    Task<T?> RpcAsync<T>(string functionName, object? args, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> RpcListAsync<T>(string functionName, object? args, CancellationToken cancellationToken = default);

    Task RpcExecuteAsync(string functionName, object? args, CancellationToken cancellationToken = default);
}
