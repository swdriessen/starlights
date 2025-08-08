using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Abilities;

public record AbilityScoreCreated : CharacterEventBase
{
    public AbilityScoreId AbilityScoreId { get; init; }
}
