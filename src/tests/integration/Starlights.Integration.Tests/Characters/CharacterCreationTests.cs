using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Tests.Core;
using Starlights.Modules.Characters.Endpoints.Characters.CreateCharacter;
using Starlights.Modules.Characters.Endpoints.Generation.CreationOptions;
using Starlights.Modules.Characters.Endpoints.Generation.PortraitOptions;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class CharacterCreationTests : IntegrationTestBase
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

        await client.GetAsync("/api/elements/initialize", TestCancellationToken);

        var response = await client.GetAsync("/api/characters/creation-options", TestCancellationToken);
        var responseJson = await response.Content.ReadFromJsonAsync<GetCharacterCreationOptionsResponse>(TestCancellationToken);
        responseJson?.Options.Should().NotBeEmpty("expected at least one character creation option to be available");

        _options.AddRange(responseJson!.Options);

        var portraitResponse = await client.GetAsync("/api/characters/portrait-options", TestCancellationToken);
        var portraitResponseJson = await portraitResponse.Content.ReadFromJsonAsync<GetCharacterPortraitOptionsResponse>(TestCancellationToken);
        portraitResponseJson?.Portraits.Should().NotBeEmpty("expected at least one character portrait option to be available");

        _portraits.AddRange(portraitResponseJson!.Portraits);
    }

    [TestMethod]
    public async Task CreateCharacterFromCharacterCreationElement()
    {
        // Arrange
        var client = _integration.CreateClient();

        var request = new CreateCharacterRequest(_options[0].Id, "Test Character", _portraits[0]?.Url);

        // Act
        var response = await client.PostAsJsonAsync("/api/characters", request, TestCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
