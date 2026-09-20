using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MiProyecto.IntegrationTests.Support;

internal static class TestJwt
{
    public const string Issuer = "MiProyecto.Tests";
    public const string Audience = "MiProyecto.API";
    public const string SigningKey = "INTEGRATION_TEST_SIGNING_KEY_32CHARS!!";

    public static string CreateToken(params string[] roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "1"),
            new("sub", "1")
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            Issuer,
            Audience,
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
