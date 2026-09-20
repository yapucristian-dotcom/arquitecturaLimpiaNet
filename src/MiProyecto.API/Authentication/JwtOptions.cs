namespace MiProyecto.API.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string? Authority { get; set; }

    public string Issuer { get; set; } = "MiProyecto";

    public string Audience { get; set; } = "MiProyecto.API";

    public bool RequireHttpsMetadata { get; set; } = true;

    public string? SigningKey { get; set; }
}
