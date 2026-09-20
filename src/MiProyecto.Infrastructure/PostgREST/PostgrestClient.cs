using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiProyecto.Application.Common.Exceptions;

namespace MiProyecto.Infrastructure.PostgREST;

public sealed class PostgrestClient : IPostgrestClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient;
    private readonly PostgrestOptions _options;
    private readonly IPostgrestRequestContext _requestContext;
    private readonly ILogger<PostgrestClient> _logger;

    public PostgrestClient(
        HttpClient httpClient,
        IOptions<PostgrestOptions> options,
        IPostgrestRequestContext requestContext,
        ILogger<PostgrestClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _requestContext = requestContext;
        _logger = logger;
    }

    public async Task<T?> RpcAsync<T>(string functionName, object? args, CancellationToken cancellationToken = default)
    {
        var json = await SendRpcAsync(functionName, args, cancellationToken);

        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        try
        {
            return DeserializeSingle<T>(json);
        }
        catch (JsonException exception)
        {
            _logger.LogError(exception, "No fue posible interpretar la respuesta de PostgREST en {Function}", functionName);
            throw new InfrastructureException(
                "No fue posible interpretar la respuesta de PostgREST.",
                innerException: exception);
        }
    }

    public async Task<IReadOnlyList<T>> RpcListAsync<T>(string functionName, object? args, CancellationToken cancellationToken = default)
    {
        var json = await SendRpcAsync(functionName, args, cancellationToken);

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        try
        {
            return DeserializeList<T>(json);
        }
        catch (JsonException exception)
        {
            _logger.LogError(exception, "No fue posible interpretar la respuesta de PostgREST en {Function}", functionName);
            throw new InfrastructureException(
                "No fue posible interpretar la respuesta de PostgREST.",
                innerException: exception);
        }
    }

    public async Task RpcExecuteAsync(string functionName, object? args, CancellationToken cancellationToken = default)
    {
        await SendRpcAsync(functionName, args, cancellationToken);
    }

    private async Task<string> SendRpcAsync(string functionName, object? args, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"rpc/{functionName}");
        request.Content = new StringContent(
            JsonSerializer.Serialize(args ?? new { }, JsonOptions),
            Encoding.UTF8,
            "application/json");

        ApplyHeaders(request);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "PostgREST RPC {Function} {StatusCode} en {ElapsedMs} ms",
                    functionName,
                    (int)response.StatusCode,
                    stopwatch.ElapsedMilliseconds);

                return body;
            }

            var error = TryParseError(body);
            var exception = new PostgrestException(
                error?.Message ?? "PostgREST devolvió un error.",
                (int)response.StatusCode,
                error?.Code,
                error?.Details,
                error?.Hint);

            _logger.LogWarning(
                exception,
                "PostgREST RPC {Function} falló con {StatusCode} {Code} en {ElapsedMs} ms",
                functionName,
                exception.HttpStatus,
                exception.Code,
                stopwatch.ElapsedMilliseconds);

            throw PostgrestErrorTranslator.ToApplicationException(exception);
        }
        catch (AppException)
        {
            throw;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException exception)
        {
            _logger.LogWarning(exception, "PostgREST RPC {Function} superó el tiempo de espera", functionName);
            throw new InfrastructureException(
                "La solicitud a PostgREST superó el tiempo de espera.",
                innerException: exception);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "No fue posible comunicarse con PostgREST en {Function}", functionName);
            throw new InfrastructureException(
                "No fue posible comunicarse con PostgREST.",
                innerException: exception);
        }
        catch (JsonException exception)
        {
            _logger.LogError(exception, "No fue posible interpretar la respuesta de PostgREST en {Function}", functionName);
            throw new InfrastructureException(
                "No fue posible interpretar la respuesta de PostgREST.",
                innerException: exception);
        }
    }

    private void ApplyHeaders(HttpRequestMessage request)
    {
        var correlationId = string.IsNullOrWhiteSpace(_requestContext.CorrelationId)
            ? Guid.NewGuid().ToString("N")
            : _requestContext.CorrelationId;

        request.Headers.TryAddWithoutValidation("X-Correlation-ID", correlationId);

        var token = _requestContext.AccessToken ?? _options.Jwt;
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            request.Headers.TryAddWithoutValidation(_options.ApiKeyHeader, _options.ApiKey);
        }
    }

    private static PostgrestErrorResponse? TryParseError(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<PostgrestErrorResponse>(body, JsonOptions);
        }
        catch (JsonException)
        {
            return new PostgrestErrorResponse { Message = body };
        }
    }

    private static T? DeserializeSingle<T>(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        if (root.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return default;
        }

        if (root.ValueKind == JsonValueKind.Array)
        {
            if (root.GetArrayLength() == 0)
            {
                return default;
            }

            return DeserializeElement<T>(root[0]);
        }

        return DeserializeElement<T>(root);
    }

    private static IReadOnlyList<T> DeserializeList<T>(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        if (root.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return [];
        }

        if (root.ValueKind == JsonValueKind.Array)
        {
            return root.Deserialize<List<T>>(JsonOptions) ?? [];
        }

        var item = root.Deserialize<T>(JsonOptions);
        return item is null ? [] : [item];
    }

    private static T? DeserializeElement<T>(JsonElement element)
    {
        if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out var scalar))
            {
                return (T)(object)scalar;
            }

            if (element.ValueKind == JsonValueKind.Object &&
                element.TryGetProperty("id", out var idProperty) &&
                idProperty.TryGetInt64(out var id))
            {
                return (T)(object)id;
            }
        }

        return element.Deserialize<T>(JsonOptions);
    }
}
