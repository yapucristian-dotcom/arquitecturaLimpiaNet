using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiProyecto.Application.Abstractions;
using MiProyecto.Infrastructure.Database;
using MiProyecto.Infrastructure.Persistence;
using MiProyecto.Infrastructure.PostgREST;
using MiProyecto.Infrastructure.SqlServer;

namespace MiProyecto.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration.GetSection(DatabaseOptions.SectionName)[nameof(DatabaseOptions.Provider)];

        services
            .AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Validate(options => DatabaseProviders.IsKnown(options.Provider), "Database:Provider debe ser PostgREST o SqlServer.")
            .Validate(
                options => !DatabaseProviders.IsSqlServer(options.Provider) || !string.IsNullOrWhiteSpace(options.ConnectionString),
                "Database:ConnectionString es obligatorio en appsettings.json cuando Provider es SqlServer.")
            .Validate(
                options => !DatabaseProviders.IsSqlServer(options.Provider) || options.CommandTimeoutSeconds > 0,
                "Database:CommandTimeoutSeconds debe ser mayor que cero.")
            .ValidateOnStart();

        if (DatabaseProviders.IsSqlServer(provider))
        {
            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IUsuarioRepository, SqlUsuarioRepository>();
            return services;
        }

        services
            .AddOptions<PostgrestOptions>()
            .Bind(configuration.GetSection(PostgrestOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.BaseUrl), "PostgREST:BaseUrl es obligatorio.")
            .Validate(options => options.TimeoutSeconds > 0, "PostgREST:TimeoutSeconds debe ser mayor que cero.")
            .ValidateOnStart();

        services.AddScoped<IPostgrestRequestContext, PostgrestRequestContext>();

        services.AddHttpClient<IPostgrestClient, PostgrestClient>((serviceProvider, client) =>
        {
            var options = configuration
                .GetSection(PostgrestOptions.SectionName)
                .Get<PostgrestOptions>() ?? new PostgrestOptions();

            client.BaseAddress = CreateBaseAddress(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        return services;
    }

    private static Uri CreateBaseAddress(string baseUrl)
    {
        return new Uri(baseUrl.Trim().TrimEnd('/') + "/");
    }
}
