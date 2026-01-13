using AwesomeAssertions;
using Starlights.Modules.Characters.Endpoints.Characters.Skills;

namespace Starlights.Integration.Drivers.CharacterCreation;

internal sealed class SkillsDriver : IDriver
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