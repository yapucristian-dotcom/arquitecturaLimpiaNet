using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MiProyecto.API.Authentication;

namespace MiProyecto.API.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? jwt;
                options.RequireHttpsMetadata = jwt.RequireHttpsMetadata;
                options.Audience = jwt.Audience;

                if (!string.IsNullOrWhiteSpace(jwt.Authority))
                {
                    options.Authority = jwt.Authority;
                    options.TokenValidationParameters.ValidAudience = jwt.Audience;
                    options.TokenValidationParameters.ValidateAudience = !string.IsNullOrWhiteSpace(jwt.Audience);
                    return;
                }

                var signingKey = jwt.SigningKey;
                if (string.IsNullOrWhiteSpace(signingKey))
                {
                    if (!environment.IsDevelopment() && !environment.IsEnvironment("Testing"))
                    {
                        throw new InvalidOperationException(
                            "Jwt:SigningKey o Jwt:Authority es obligatorio. Use User Secrets o la variable Jwt__SigningKey.");
                    }

                    signingKey = "DEV_ONLY_DO_NOT_USE_IN_PRODUCTION_CHANGE_ME";
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.Admin, policy => policy.RequireRole("Admin"));
            options.AddPolicy(AuthorizationPolicies.User, policy => policy.RequireRole("Admin", "User"));
        });

        return services;
    }
}
