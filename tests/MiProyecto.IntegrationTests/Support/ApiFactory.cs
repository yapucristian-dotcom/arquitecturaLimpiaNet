using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiProyecto.Application.Abstractions;
using NSubstitute;

namespace MiProyecto.IntegrationTests.Support;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public IUsuarioRepository Usuarios { get; } = Substitute.For<IUsuarioRepository>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = TestJwt.Issuer,
                ["Jwt:Audience"] = TestJwt.Audience,
                ["Jwt:SigningKey"] = TestJwt.SigningKey,
                ["Jwt:RequireHttpsMetadata"] = "false",
                ["PostgREST:BaseUrl"] = "http://localhost:3000",
                ["PostgREST:TimeoutSeconds"] = "5",
                ["Cors:AllowedOrigins:0"] = "http://localhost:5173",
                ["Swagger:Enabled"] = "false"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            foreach (var descriptor in services.Where(service => service.ServiceType == typeof(IUsuarioRepository)).ToList())
            {
                services.Remove(descriptor);
            }

            services.AddSingleton(Usuarios);
        });
    }

    public HttpClient CreateAuthenticatedClient(params string[] roles)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TestJwt.CreateToken(roles));

        return client;
    }
}
