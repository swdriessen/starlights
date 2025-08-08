using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Domain.Characters.Eventing;

public record CharacterEventBase : EventBase
{
    public required Guid CharacterId { get; init; }
}
