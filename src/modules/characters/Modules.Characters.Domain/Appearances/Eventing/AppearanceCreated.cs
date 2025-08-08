using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Appearances.Eventing;

public record AppearanceCreated : CharacterEventBase
{
    public required Guid AppearanceId { get; init; }
}

