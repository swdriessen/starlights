using FastEndpoints;

namespace Starlights.Modules.Characters.Endpoints.Generation.AbilityScores.GetAbilities;

internal sealed class GetAbilityScoresRequest
{
    [BindFrom("id")]
    public Guid CharacterId { get; set; }
}
