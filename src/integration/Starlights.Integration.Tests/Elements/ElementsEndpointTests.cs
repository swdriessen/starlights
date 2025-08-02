using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Modules.Elements.Integration.Models;

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
    [DataRow("/api/elements/types/character-creation")]
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

    [TestMethod]
    public async Task GetCharacterCreationTypes()
    {
        // Arrange
        var client = _integration.CreateClient();
        _ = await client.GetAsync("/api/elements/initialize", CancellationToken.None);

        // Act
        var response = await client.GetAsync("/api/elements/types/character-creation", CancellationToken.None);

        // Assert
        var info = await response.Content.ReadFromJsonAsync<List<CharacterCreationInfo>>(CancellationToken.None);
        info.Should().HaveCountGreaterThan(0, "expected at least one character creation type");
    }
}
