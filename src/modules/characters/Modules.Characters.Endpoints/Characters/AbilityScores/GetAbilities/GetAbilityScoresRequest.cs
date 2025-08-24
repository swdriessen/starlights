using FastEndpoints;

namespace Starlights.Modules.Characters.Endpoints.Characters.AbilityScores.GetAbilities;

internal sealed class GetAbilityScoresRequest
{
    [BindFrom("id")]
    public Guid CharacterId { get; set; }
}
