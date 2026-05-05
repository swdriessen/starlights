using AwesomeAssertions;
using Starlights.Integration.Drivers.Characters.Endpoints;
using Starlights.Modules.Characters.Endpoints.Models;

namespace Starlights.Integration.Drivers.Characters.Manage;

internal sealed class AbilityScoreDriver : IDriver
{
    private readonly IIntegrationHost _host;
    private readonly AbilityScoresEndpointDriver _api;
    private readonly CharactersDriverContext _context;

    public AbilityScoreDriver(IIntegrationHost host, AbilityScoresEndpointDriver api, CharactersDriverContext context)
    {
        _host = host;
        _api = api;
        _context = context;
    }

    public async Task<List<AbilityScoreDataModel>> GetAbilityScores()
    {
        var response = await _api.GetAbilityScoresAsync(_context.CurrentCharacter.Id);
        response.AbilityScores.Should().NotBeNull();
        return response.AbilityScores;
    }

    public async Task<AbilityScoreDataModel> GetAbilityScore(string name)
    {
        var abilities = await GetAbilityScores();
        abilities.Should().ContainSingle(x => x.Name == name, $"expected to find ability score with name '{name}'");
        return abilities.Single(a => a.Name == name);
    }

    public async Task UpdateAbilityScoreBase(Guid abilityScoreId, int newBaseValue)
    {
        await _api.UpdateBaseScoreAsync(_context.CurrentCharacter.Id, abilityScoreId, newBaseValue);
    }
}