using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Starlights.Integration.Core;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class ElementsEndpointTests
{
    private readonly IntegrationHost _integration;

    public ElementsEndpointTests()
    {
        _integration = IntegrationHost.CreateBuilder()
            .Build();
    }

    [DataRow("/api/elements/initialize")]
    [TestMethod]
    public async Task EnsureSuccessStatusCode(string url)
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetAsync(url, CancellationToken.None);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public async Task InitializeElements()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetAsync("/api/elements/initialize", CancellationToken.None);

        // Assert
        var json = await response.Content.ReadFromJsonAsync<object>(CancellationToken.None);
        json.Should().NotBeNull("expected response content to be deserializable");
    }
}
