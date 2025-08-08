using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Abilities;

public record AbilityScoreRemoved : CharacterEventBase
{
    public AbilityScoreId AbilityScoreId { get; init; }
}