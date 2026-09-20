namespace MiProyecto.API.Extensions;

public static class CorsExtensions
{
    public const string PolicyName = "Default";

    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                if (origins.Length == 0)
                {
                    policy.SetIsOriginAllowed(_ => false);
                    return;
                }

                policy
                    .WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
