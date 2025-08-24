using FastEndpoints;

namespace Starlights.Modules.Characters.Endpoints.Characters.AbilityScores.UpdateAdditionalScore;

internal sealed class UpdateAbilityAdditionalScoreRequest
{
    [BindFrom("characterId")]
    public Guid CharacterId { get; set; }

    [BindFrom("abilityScoreId")]
    public Guid AbilityScoreId { get; set; }

    public int Value { get; set; }
}
