using MiProyecto.API.Extensions;
using MiProyecto.API.Middleware;
using MiProyecto.Application;
using MiProyecto.Application.Common.Errors;
using MiProyecto.Application.Common.Responses;
using MiProyecto.Infrastructure;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "MiProyecto.API"));

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApiAuthentication(builder.Configuration, builder.Environment);
    builder.Services.AddApiCors(builder.Configuration);
    builder.Services.AddApiSwagger();

    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(entry => entry.Value is { Errors.Count: > 0 })
                    .SelectMany(entry => entry.Value!.Errors.Select(error => new ApiError
                    {
                        Field = ToCamelCase(entry.Key),
                        Message = string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "El valor no es válido."
                            : error.ErrorMessage
                    }))
                    .ToArray();

                var correlationId = context.HttpContext.GetCorrelationId();
                return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(
                    ApiResponse.Fail("Se encontraron errores de validación", ErrorCodes.ValidationError, errors, correlationId));
            };
        });

    var app = builder.Build();

    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("CorrelationId", httpContext.GetCorrelationId());
            diagnosticContext.Set("UserId", httpContext.GetUserId());
            diagnosticContext.Set("RequestPath", httpContext.Request.Path.Value);
            diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);
        };

        options.GetLevel = (httpContext, _, exception) =>
        {
            if (exception is not null || httpContext.Response.StatusCode >= 500)
            {
                return LogEventLevel.Error;
            }

            if (httpContext.Response.StatusCode >= 400 || !app.Environment.IsDevelopment())
            {
                return LogEventLevel.Warning;
            }

            return LogEventLevel.Information;
        };
    });
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseApiSwagger();

    if (!app.Environment.IsEnvironment("Testing"))
    {
        app.UseHttpsRedirection();
    }
    app.UseCors(CorsExtensions.PolicyName);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "La API no pudo iniciar.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

static string ToCamelCase(string value)
{
    if (string.IsNullOrEmpty(value) || char.IsLower(value[0]))
    {
        return value;
    }

    return char.ToLowerInvariant(value[0]) + value[1..];
}

public partial class Program;
