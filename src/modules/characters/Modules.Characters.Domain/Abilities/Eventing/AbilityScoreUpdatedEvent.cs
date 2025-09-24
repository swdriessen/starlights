using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Abilities.Eventing;

public record AbilityScoreUpdatedEvent : CharacterEventBase
{
    public AbilityScoreId AbilityScoreId { get; init; }
    public required int NewAbilityScoreValue { get; init; }
    public required int NewAbilityModifier { get; init; }
}
