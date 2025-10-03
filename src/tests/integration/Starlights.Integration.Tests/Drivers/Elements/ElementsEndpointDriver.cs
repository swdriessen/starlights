using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Core;

namespace Starlights.Integration.Drivers.Elements;

internal class ElementsEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ElementsEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    public async Task InitializeElementsAsync(CancellationToken cancellation)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync("/api/elements/initialize", cancellation);
        response.EnsureSuccessStatusCode();

        response.Content.ReadFromJsonAsync<object>(cancellation)
            .Should().NotBeNull("expected response content to be deserializable");
    }
}
