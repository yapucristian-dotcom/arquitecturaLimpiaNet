namespace MiProyecto.Infrastructure.Database;

public static class DatabaseProviders
{
    public const string Postgrest = "PostgREST";

    public const string SqlServer = "SqlServer";

    public static bool IsSqlServer(string? provider) =>
        string.Equals(provider?.Trim(), SqlServer, StringComparison.OrdinalIgnoreCase);

    public static bool IsKnown(string? provider) =>
        string.IsNullOrWhiteSpace(provider)
        || string.Equals(provider.Trim(), Postgrest, StringComparison.OrdinalIgnoreCase)
        || IsSqlServer(provider);
}
