using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacterClasses;
using Starlights.Modules.Characters.Endpoints.Characters.LevelUp;
using Starlights.Modules.Characters.Endpoints.CharacterSheet.GetFeatures;

namespace Starlights.Integration.Drivers.CharacterCreation.Endpoints;

internal sealed class CharacterManagementEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public CharacterManagementEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    public async Task<GetCharacterClassesResponse> GetCharacterClasses(Guid characterId)
    {
        using var api = _integration.CreateClient();

        var url = $"/api/characters/{characterId}/classes";
        var response = await api.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetCharacterClassesResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }


    public async Task UpdateClassLevel(Guid characterId, Guid classId, int newLevel)
    {
        using var api = _integration.CreateClient();

        var url = $"/api/characters/{characterId}/classes/{classId}/level";
        var payload = new UpdateClassLevelRequest() { NewLevel = newLevel };

        var response = await api.PostAsJsonAsync(url, payload);
        response.EnsureSuccessStatusCode();
    }

    public async Task<GetFeaturesResponse> GetFeatures(Guid characterId)
    {
        using var api = _integration.CreateClient();

        var url = $"/api/characters/{characterId}/features";

        var response = await api.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetFeaturesResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }
}
