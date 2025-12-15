using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Modules.Characters.Endpoints.Characters.AbilityScores.GetAbilities;
using Starlights.Modules.Characters.Endpoints.Characters.AbilityScores.UpdateAdditionalScore;
using Starlights.Modules.Characters.Endpoints.Characters.AbilityScores.UpdateBaseScore;

namespace Starlights.Integration.Drivers.CharacterCreation;

internal sealed class AbilityScoresEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public AbilityScoresEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    /// <summary>
    /// Retrieves the ability scores for the current character.
    /// </summary>
    public async Task<GetAbilityScoresResponse> GetAbilityScores()
    {
        using var api = _integration.CreateClient();

        var characterId = _integration.GetCharacterIdentifier();
        var url = $"/api/characters/{characterId}/ability-scores";

        var response = await api.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetAbilityScoresResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }

    /// <summary>
    /// Updates the base score of a specified ability for the current character.
    /// </summary>
    public async Task<UpdateAbilityBaseScoreResponse> UpdateBaseScore(Guid abilityScoreId, int value)
    {
        using var api = _integration.CreateClient();

        var characterId = _integration.GetCharacterIdentifier();

        var url = $"/api/characters/{characterId}/ability-scores/{abilityScoreId}/base";
        var payload = new UpdateAbilityBaseScoreRequest { Value = value };

        var response = await api.PostAsJsonAsync(url, payload);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<UpdateAbilityBaseScoreResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }

    /// <summary>
    /// Updates the additional score of a specified ability for the current character.
    /// </summary>
    public async Task<UpdateAbilityAdditionalScoreResponse> UpdateAdditionalScore(Guid abilityScoreId, int value)
    {
        using var api = _integration.CreateClient();

        var characterId = _integration.GetCharacterIdentifier();

        var url = $"/api/characters/{characterId}/ability-scores/{abilityScoreId}/additional";
        var payload = new UpdateAbilityAdditionalScoreRequest { Value = value };

        var response = await api.PostAsJsonAsync(url, payload);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<UpdateAbilityAdditionalScoreResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }
}
