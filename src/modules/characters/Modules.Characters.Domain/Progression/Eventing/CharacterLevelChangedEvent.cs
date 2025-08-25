using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Progression.Eventing;

public record CharacterLevelChangedEvent : CharacterEventBase
{
    public required int NewLevel { get; init; }
}

