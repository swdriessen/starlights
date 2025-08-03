using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Tests.Core;
using Starlights.Modules.Characters.Endpoints.Entities.Characters.Create;
using Starlights.Modules.Characters.Endpoints.Generation.CreationOptions;
using Starlights.Modules.Characters.Endpoints.Generation.PortraitOptions;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class CharacterCreationTests
{
    private readonly IntegrationHost _integration;
    private readonly List<CharacterCreationOption> _options = [];
    private readonly List<CharacterPortraitOption> _portraits = [];

    public CharacterCreationTests()
    {
        _integration = IntegrationHost.CreateBuilder()
            .Build();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        var client = _integration.CreateClient();

        await client.GetAsync("/api/elements/initialize", CancellationToken.None);

        var response = await client.GetAsync("/api/characters/creation-options", CancellationToken.None);
        var responseJson = await response.Content.ReadFromJsonAsync<GetCharacterCreationOptionsResponse>(CancellationToken.None);
        responseJson?.Options.Should().NotBeEmpty("expected at least one character creation option to be available");

        _options.AddRange(responseJson!.Options);

        var portraitResponse = await client.GetAsync("/api/characters/portrait-options", CancellationToken.None);
        var portraitResponseJson = await portraitResponse.Content.ReadFromJsonAsync<GetCharacterPortraitOptionsResponse>(CancellationToken.None);
        portraitResponseJson?.Portraits.Should().NotBeEmpty("expected at least one character portrait option to be available");

        _portraits.AddRange(portraitResponseJson!.Portraits);
    }

    [TestMethod]
    public async Task CreateCharacterFromCharacterCreationElement()
    {
        // Arrange
        var client = _integration.CreateClient();

        var request = new CreateCharacterRequest
        {
            // CharacterCreationId
            Name = "Test Character"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/characters/create", request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
