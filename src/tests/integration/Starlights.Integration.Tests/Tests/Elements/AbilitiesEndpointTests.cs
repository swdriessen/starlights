using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Entities.Abilities.GetAbilities;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class AbilitiesEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;

    [TestInitialize]
    public void Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
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
        var response = await client.PostAsJsonAsync("/api/elements/abilities/create", request, _integration.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await response.Content.ReadFromJsonAsync<object>(_integration.CancellationToken);
        json.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetAbilities()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetAsync("/api/elements/abilities", _integration.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<GetAbilitiesResponse>(_integration.CancellationToken);
        json?.Abilities.Should().NotBeNull();
    }
}
