using System.Text.Json;
using MiProyecto.API.Extensions;
using MiProyecto.Application.Common.Errors;
using MiProyecto.Application.Common.Exceptions;
using MiProyecto.Application.Common.Responses;
using MiProyecto.Domain.Exceptions;

namespace MiProyecto.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await WriteErrorAsync(context, exception);
        }
    }

    private async Task WriteErrorAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.GetCorrelationId();
        var (statusCode, response) = MapException(exception, correlationId);

        if (statusCode >= 500)
        {
            _logger.LogError(exception, "Error no controlado. CorrelationId {CorrelationId}", correlationId);
        }
        else
        {
            _logger.LogWarning(exception, "Error de aplicación {StatusCode}. CorrelationId {CorrelationId}", statusCode, correlationId);
        }

        if (context.Response.HasStarted)
        {
            throw exception;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    private (int StatusCode, ApiResponse<object?> Response) MapException(Exception exception, string correlationId)
    {
        return exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                ApiResponse.Fail(
                    validationException.Message,
                    validationException.Code,
                    validationException.Errors,
                    correlationId)),
            NotFoundException notFoundException => (
                StatusCodes.Status404NotFound,
                ApiResponse.Fail(notFoundException.Message, notFoundException.Code, correlationId: correlationId)),
            ConflictException conflictException => (
                StatusCodes.Status409Conflict,
                ApiResponse.Fail(conflictException.Message, conflictException.Code, correlationId: correlationId)),
            InfrastructureException infrastructureException => (
                StatusCodes.Status503ServiceUnavailable,
                ApiResponse.Fail(infrastructureException.Message, infrastructureException.Code, correlationId: correlationId)),
            AppException appException => (
                MapAppStatusCode(appException.Code),
                ApiResponse.Fail(appException.Message, appException.Code, appException.Errors, correlationId)),
            DomainException domainException => (
                StatusCodes.Status400BadRequest,
                ApiResponse.Fail(domainException.Message, ErrorCodes.ValidationError, correlationId: correlationId)),
            _ => (
                StatusCodes.Status500InternalServerError,
                ApiResponse.Fail(
                    _environment.IsDevelopment() || _environment.IsEnvironment("Testing")
                        ? exception.Message
                        : "Ocurrió un error interno.",
                    ErrorCodes.Internal,
                    correlationId: correlationId))
        };
    }

    private static int MapAppStatusCode(string code) => code switch
    {
        ErrorCodes.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorCodes.Forbidden => StatusCodes.Status403Forbidden,
        ErrorCodes.NotFound => StatusCodes.Status404NotFound,
        ErrorCodes.Conflict => StatusCodes.Status409Conflict,
        ErrorCodes.InfraUnavailable => StatusCodes.Status503ServiceUnavailable,
        _ => StatusCodes.Status400BadRequest
    };
}
