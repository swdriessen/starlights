using Starlights.Modules.Characters.Endpoints.Generation.AbilityScores;

namespace Starlights.Modules.Characters.Endpoints.Generation.AbilityScores.GetAbilities;

internal sealed class GetAbilityScoresResponse
{
    public List<AbilityScoreDataModel> AbilityScores { get; set; } = [];
}
