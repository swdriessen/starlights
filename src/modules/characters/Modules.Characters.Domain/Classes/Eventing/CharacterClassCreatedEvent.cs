using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Classes.Eventing;

/// <summary>
/// Event raised when a character class is created for a character.
/// </summary>
public sealed record CharacterClassCreatedEvent : CharacterEventBase
{
    public CharacterClassId ClassId { get; init; }
}
