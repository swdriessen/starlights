using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Tests.Core;
using Starlights.Modules.Elements.Endpoints.Entities.Abilities.GetAbilities;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class AbilitiesEndpointTests
{
    private readonly IntegrationHost _integration;

    public AbilitiesEndpointTests()
    {
        _integration = IntegrationHost.CreateBuilder()
            .Build();
    }

    [TestMethod]
    public async Task CreateAbility()
    {
        // Arrange
        var client = _integration.CreateClient();
        var request = new
        {
            name = "Intelligence",
            abbreviation = "INT"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/elements/abilities/create", request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await response.Content.ReadFromJsonAsync<object>(cancellationToken: CancellationToken.None);
        json.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetAbilities()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetAsync("/api/elements/abilities", CancellationToken.None);

        // Assert
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<GetAbilitiesResponse>(cancellationToken: CancellationToken.None);
        json?.Abilities.Should().NotBeNull();
    }
}
