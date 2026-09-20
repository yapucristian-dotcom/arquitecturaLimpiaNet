using System.Security.Claims;

namespace MiProyecto.API.Extensions;

public static class HttpContextExtensions
{
    public const string CorrelationHeaderName = "X-Correlation-ID";
    public const string CorrelationItemKey = "CorrelationId";

    public static string GetCorrelationId(this HttpContext context)
    {
        if (context.Items.TryGetValue(CorrelationItemKey, out var value) && value is string correlationId)
        {
            return correlationId;
        }

        return context.TraceIdentifier;
    }

    public static string? GetUserId(this HttpContext context)
    {
        return context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue("sub");
    }
}
