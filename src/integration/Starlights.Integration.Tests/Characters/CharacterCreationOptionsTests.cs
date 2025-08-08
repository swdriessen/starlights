using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Tests.Core;
using Starlights.Modules.Characters.Endpoints.Generation.CreationOptions;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class CharacterCreationOptionsTests
{
    private readonly IntegrationHost _integration;

    public CharacterCreationOptionsTests()
    {
        _integration = IntegrationHost.CreateBuilder()
            .Build();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        var client = _integration.CreateClient();

        await client.GetAsync("/api/elements/initialize", CancellationToken.None);
    }

    [TestMethod]
    public async Task GetCharacterCreationOptions()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetAsync("/api/characters/creation-options", CancellationToken.None);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadFromJsonAsync<GetCharacterCreationOptionsResponse>(CancellationToken.None);
        responseJson?.Options.Should().NotBeEmpty();
    }
}
