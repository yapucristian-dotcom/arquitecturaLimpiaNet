using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiProyecto.Application.Abstractions;
using MiProyecto.Infrastructure.Persistence;
using MiProyecto.Infrastructure.PostgREST;

namespace MiProyecto.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
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
