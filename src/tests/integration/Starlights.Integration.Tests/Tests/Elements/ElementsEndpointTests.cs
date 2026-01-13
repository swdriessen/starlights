using System.Net.Http.Json;
using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Starlights.Integration.Extensions;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class ElementsEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;

    [TestInitialize]
    public void Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();
    }

    [DataRow("/api/elements/initialize")]
    [TestMethod]
    public async Task EnsureSuccessStatusCode(string url)
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetAsync(url, _integration.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public async Task InitializeElements()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetAsync("/api/elements/initialize", _integration.CancellationToken);

        // Assert
        var json = await response.Content.ReadFromJsonAsync<object>(_integration.CancellationToken);
        json.Should().NotBeNull("expected response content to be deserializable");
    }
}
