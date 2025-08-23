using FastEndpoints;

namespace Starlights.Modules.Characters.Endpoints.Generation.AbilityScores.UpdateBaseScore;

internal sealed class UpdateAbilityBaseScoreRequest
{
    [BindFrom("characterId")]
    public Guid CharacterId { get; set; }

    [BindFrom("abilityScoreId")]
    public Guid AbilityScoreId { get; set; }

    public int Value { get; set; }
}
