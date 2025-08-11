using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Abilities.Eventing;

public record AbilityScoreCreatedEvent : CharacterEventBase
{
    public AbilityScoreId AbilityScoreId { get; init; }
}

public record AbilityScoreUpdatedEvent : CharacterEventBase
{
    public AbilityScoreId AbilityScoreId { get; init; }
}
