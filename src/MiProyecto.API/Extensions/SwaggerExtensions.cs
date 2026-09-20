using Microsoft.OpenApi;
using MiProyecto.API.Authentication;

namespace MiProyecto.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddApiSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "MiProyecto API",
                Version = "v1",
                Description = "API corporativa Clean Architecture + PostgREST."
            });

            var bearerScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Ingrese el token JWT. Ejemplo: Bearer {token}"
            };

            options.AddSecurityDefinition("Bearer", bearerScheme);
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer")] = []
            });
        });

        return services;
    }

    public static IApplicationBuilder UseApiSwagger(this WebApplication app)
    {
        var enabled = app.Environment.IsDevelopment()
            || app.Configuration.GetValue("Swagger:Enabled", false);

        if (!enabled)
        {
            return app;
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "MiProyecto API v1");
        });

        return app;
    }
}
