using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Characters.Endpoints.Characters.CreateCharacter;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacterDetails;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;
using Starlights.Modules.Characters.Endpoints.Generation.CreationOptions;
using Starlights.Modules.Characters.Endpoints.Generation.PortraitOptions;

namespace Starlights.Integration.Drivers.Characters.Endpoints;

public sealed class CharactersEndpointDriver : IDriver
{
    private readonly IIntegrationHost _host;

    public CharactersEndpointDriver(IIntegrationHost host)
    {
        _host = host;
    }

    /// <summary>
    /// Creates a new character using the specified creation option, name, and optional portrait path.
    /// </summary>
    public async Task<Guid> CreateCharacterAsync(Guid creationOption, string name, string? portraitPath)
    {
        using var api = _host.CreateClient();

        var payload = new CreateCharacterRequest(creationOption, name, portraitPath);

        var response = await api.PostAsJsonAsync("/api/characters", payload, _host.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, "Character creation should return HTTP 201 Created.");

        var data = await response.Content.ReadFromJsonAsync<CreateCharacterResponse>(_host.CancellationToken);
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data.Id;
    }

    /// <summary>
    /// Deletes the character with the specified identifier.
    /// </summary>
    public async Task DeleteCharacterAsync(Guid characterId)
    {
        using var api = _host.CreateClient();

        var url = $"/api/characters/{characterId}";

        var response = await api.DeleteAsync(url, _host.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent, "Character deletion should return HTTP 204 No Content.");
    }

    /// <summary>
    /// Retrieves a list of all characters associated with the current user.
    /// </summary>
    public async Task<GetCharactersResponse> GetCharactersAsync()
    {
        using var api = _host.CreateClient();

        var url = $"/api/characters";

        var response = await api.GetAsync(url, _host.CancellationToken);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetCharactersResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }

    /// <summary>
    /// Retrieves the details of a specific character by its ID.
    /// </summary>
    public async Task<GetCharacterDetailsResponse> GetCharacterAsync(Guid characterId)
    {
        var url = $"/api/characters/{characterId}";

        using var api = _host.CreateClient();

        var response = await api.GetAsync(url, _host.CancellationToken);
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
        using var api = _host.CreateClient();

        var response = await api.GetAsync("/api/characters/creation-options", _host.CancellationToken);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetCharacterCreationOptionsResponse>(_host.CancellationToken);
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }

    /// <summary>
    /// Retrieves the available options for character portrait generation.
    /// </summary>
    public async Task<GetCharacterPortraitOptionsResponse> GetCharacterPortraitOptionsAsync()
    {
        using var api = _host.CreateClient();

        var response = await api.GetAsync("/api/characters/portrait-options", _host.CancellationToken);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetCharacterPortraitOptionsResponse>(_host.CancellationToken);
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }
}
