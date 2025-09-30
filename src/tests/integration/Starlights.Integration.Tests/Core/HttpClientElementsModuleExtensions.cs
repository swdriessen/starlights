using System.Net;
using FluentAssertions;

namespace Starlights.Integration.Tests.Core;

/// <summary>
/// Reusable HTTP client extensions for elements-related endpoints.
/// </summary>
internal static class HttpClientElementsModuleExtensions
{
    public static async Task InitializeElementsAsync(this HttpClient client, CancellationToken ct = default)
    {
        var response = await client.GetAsync("/api/elements/initialize", ct);
        response.StatusCode.Should().BeOneOf([HttpStatusCode.OK, HttpStatusCode.NoContent, HttpStatusCode.Accepted]);
    }
}
