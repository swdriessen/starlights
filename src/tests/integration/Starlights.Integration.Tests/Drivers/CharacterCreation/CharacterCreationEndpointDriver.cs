using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Core;
using Starlights.Modules.Characters.Endpoints.Characters.CreateCharacter;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacterDetails;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;
using Starlights.Modules.Characters.Endpoints.Generation.CreationOptions;
using Starlights.Modules.Characters.Endpoints.Generation.PortraitOptions;

namespace Starlights.Integration.Drivers.CharacterCreation;

internal sealed class CharacterCreationEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public CharacterCreationEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    public async Task<Guid> CreateCharacterAsync(Guid creationOption, string name, string? portraitPath)
    {
        using var api = _integration.CreateClient();

        var payload = new CreateCharacterRequest(creationOption, name, portraitPath);

        var response = await api.PostAsJsonAsync("/api/characters", payload);
        response.StatusCode.Should().Be(HttpStatusCode.Created, "Character creation should return HTTP 201 Created.");

        var data = await response.Content.ReadFromJsonAsync<CreateCharacterResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data.Id;
    }

    public async Task DeleteCharacterAsync(Guid characterId)
    {
        using var api = _integration.CreateClient();

        var url = $"/api/characters/{characterId}";

        var response = await api.DeleteAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent, "Character deletion should return HTTP 204 No Content.");
    }

    public async Task<GetCharactersResponse> GetCharactersAsync()
    {
        using var api = _integration.CreateClient();

        var url = $"/api/characters";

        var response = await api.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetCharactersResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }

    public async Task<GetCharacterDetailsResponse> GetCharacterAsync(Guid characterId)
    {
        var url = $"/api/characters/{characterId}";

        using var api = _integration.CreateClient();

        var response = await api.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetCharacterDetailsResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }

    /// <summary>
    /// Retrieves the available options for character creation.
    /// </summary>
    public async Task<GetCharacterCreationOptionsResponse> GetCharacterCreationOptionsAsync()
    {
        using var api = _integration.CreateClient();

        var response = await api.GetAsync("/api/characters/creation-options");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetCharacterCreationOptionsResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }

    public async Task<GetCharacterPortraitOptionsResponse> GetCharacterPortraitOptionsAsync()
    {
        using var api = _integration.CreateClient();

        var response = await api.GetAsync("/api/characters/portrait-options");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetCharacterPortraitOptionsResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }
}
