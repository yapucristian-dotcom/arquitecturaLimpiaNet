using System.Net.Http.Json;
using System.Text.Json;
using MiProyecto.Application.Common.Responses;

namespace MiProyecto.IntegrationTests.Support;

internal static class HttpResponseExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static Task<ApiResponse<T>?> ReadApiResponseAsync<T>(this HttpResponseMessage response)
    {
        return response.Content.ReadFromJsonAsync<ApiResponse<T>>(JsonOptions);
    }
}
