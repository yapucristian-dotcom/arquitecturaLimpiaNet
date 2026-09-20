using MiProyecto.API.Extensions;
using MiProyecto.Infrastructure.PostgREST;
using Serilog.Context;

namespace MiProyecto.API.Middleware;

public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HttpContextExtensions.CorrelationHeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N");
        }

        context.Items[HttpContextExtensions.CorrelationItemKey] = correlationId;
        context.Response.Headers[HttpContextExtensions.CorrelationHeaderName] = correlationId;

        var postgrestContext = context.RequestServices.GetService<IPostgrestRequestContext>();
        if (postgrestContext is not null)
        {
            postgrestContext.CorrelationId = correlationId;
        }

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
