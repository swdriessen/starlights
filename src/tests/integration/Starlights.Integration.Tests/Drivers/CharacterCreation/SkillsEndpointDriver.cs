using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Modules.Characters.Endpoints.Characters.Skills;
using Starlights.Modules.Characters.Endpoints.Characters.Skills.GetSkills;

namespace Starlights.Integration.Drivers.CharacterCreation;

internal sealed class SkillsEndpointDriver
{
    private readonly IIntegrationHost _integration;

    public SkillsEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    /// <summary>
    /// Retrieves the skills for the current character.
    /// </summary>
    public async Task<GetSkillsResponse> GetSkills()
    {
        using var api = _integration.CreateClient();

        var characterId = _integration.GetCharacterIdentifier();
        var url = $"/api/characters/{characterId}/skills";

        var response = await api.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetSkillsResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }
}

internal sealed class SkillsDriver
{
    private readonly IIntegrationHost _integration;
    private readonly SkillsEndpointDriver _api;

    public SkillsDriver(IIntegrationHost integration, SkillsEndpointDriver api)
    {
        _integration = integration;
        _api = api;
    }

    public async Task<List<SkillDataModel>> GetSkills()
    {
        var response = await _api.GetSkills();
        response.Skills.Should().NotBeNull();

        return response.Skills;
    }
}