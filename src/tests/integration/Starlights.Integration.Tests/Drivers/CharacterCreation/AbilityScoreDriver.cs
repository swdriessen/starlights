using AwesomeAssertions;
using Starlights.Modules.Characters.Endpoints.Models;

namespace Starlights.Integration.Drivers.CharacterCreation;

internal sealed class AbilityScoreDriver : IDriver
{
    private readonly AbilityScoresEndpointDriver _api;

    public AbilityScoreDriver(AbilityScoresEndpointDriver api)
    {
        _api = api;
    }

    public async Task<List<AbilityScoreDataModel>> GetAbilityScores()
    {
        var response = await _api.GetAbilityScores();
        response.AbilityScores.Should().NotBeNull();

        return response.AbilityScores;
    }

    public async Task<AbilityScoreDataModel> GetAbilityScore(string name)
    {
        var abilities = await GetAbilityScores();

        var abilityScore = abilities.SingleOrDefault(a => a.Name == name);
        abilityScore.Should().NotBeNull($"Expected to find ability score with name '{name}'.");

        return abilityScore;
    }

    public async Task UpdateAbilityScoreBase(Guid abilityScoreId, int newBaseValue)
    {
        await _api.UpdateBaseScore(abilityScoreId, newBaseValue);
    }

    public async Task UpdateAdditionalValue(Guid abilityScoreId, int newAdditionalValue)
    {
        await _api.UpdateAdditionalScore(abilityScoreId, newAdditionalValue);
    }
}