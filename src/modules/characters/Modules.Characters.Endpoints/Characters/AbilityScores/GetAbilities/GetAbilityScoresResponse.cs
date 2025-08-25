using Starlights.Modules.Characters.Endpoints.Models;

namespace Starlights.Modules.Characters.Endpoints.Characters.AbilityScores.GetAbilities;

internal sealed class GetAbilityScoresResponse
{
    public List<AbilityScoreDataModel> AbilityScores { get; set; } = [];
}
