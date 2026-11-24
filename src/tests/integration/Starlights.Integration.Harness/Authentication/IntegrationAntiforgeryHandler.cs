using System.Net.Http.Json;
using Starlights.Modules.Identity.Features.Antiforgery;

namespace Starlights.Integration.Authentication;

internal sealed class IntegrationAntiforgeryHandler(HttpClient tokensClient) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Method != HttpMethod.Get && request.Method != HttpMethod.Head && request.Method != HttpMethod.Options)
        {
            using var response = await tokensClient.GetAsync("/api/identity/antiforgery", cancellationToken);
            response.EnsureSuccessStatusCode();
            var tokens = await response.Content.ReadFromJsonAsync<AntiforgeryResponse>(cancellationToken)
                ?? throw new InvalidOperationException("The antiforgery response is missing.");
            var cookie = response.Headers.GetValues("Set-Cookie")
                .Single(value => value.StartsWith("Starlights.Antiforgery=", StringComparison.Ordinal))
                .Split(';')[0];
            request.Headers.Add(tokens.HeaderName, tokens.RequestToken);
            request.Headers.Add("Cookie", cookie);
        }
        return await base.SendAsync(request, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            tokensClient.Dispose();
        }
        base.Dispose(disposing);
    }
}
