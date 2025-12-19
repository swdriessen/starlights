using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;

namespace Starlights.Integration.Core;

/// <summary>
/// Reusable helpers for concise HttpClient usage in integration tests.
/// </summary>
internal static class HttpClientExtensions
{
    public static async Task<T> GetAndReadAsync<T>(this HttpClient client, string url, HttpStatusCode expected = HttpStatusCode.OK, CancellationToken ct = default)
    {
        var response = await client.GetAsync(url, ct);
        await response.ShouldHaveStatusAsync(expected);
        var body = await response.Content.ReadFromJsonAsync<T>(ct);
        body.Should().NotBeNull($"Expected JSON body of type {typeof(T).Name} from GET {url}");
        return body!;
    }

    public static async Task<TResponse> PostJsonAndReadAsync<TResponse>(this HttpClient client, string url, object payload, HttpStatusCode expected = HttpStatusCode.OK, CancellationToken ct = default)
    {
        var response = await client.PostAsJsonAsync(url, payload, ct);
        await response.ShouldHaveStatusAsync(expected);
        var body = await response.Content.ReadFromJsonAsync<TResponse>(ct);
        body.Should().NotBeNull($"Expected JSON body of type {typeof(TResponse).Name} from POST {url}");
        return body!;
    }

    public static async Task<HttpResponseMessage> PostJsonExpectAsync(this HttpClient client, string url, object payload, HttpStatusCode expected = HttpStatusCode.OK, CancellationToken ct = default)
    {
        var response = await client.PostAsJsonAsync(url, payload, ct);
        await response.ShouldHaveStatusAsync(expected);
        return response;
    }

    public static async Task ShouldHaveStatusAsync(this HttpResponseMessage response, HttpStatusCode expected)
    {
        if (response.StatusCode != expected)
        {
            var details = string.Empty;
            try
            {
                details = await response.Content.ReadAsStringAsync();
            }
            catch
            {
                // ignore
            }

            response.StatusCode.Should().Be(expected, $"Response content: {details}");
        }
    }
}
