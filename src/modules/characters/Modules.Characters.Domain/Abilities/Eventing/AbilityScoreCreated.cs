using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Abilities.Eventing;

public record AbilityScoreCreated : CharacterEventBase
{
    public AbilityScoreId AbilityScoreId { get; init; }
}
