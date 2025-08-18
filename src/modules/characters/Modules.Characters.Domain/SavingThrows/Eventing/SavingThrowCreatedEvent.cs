using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.SavingThrows.Eventing;

/// <summary>
/// Event raised when a saving throw is created for a character.
/// </summary>
public sealed record SavingThrowCreatedEvent : CharacterEventBase
{
    public SavingThrowId SavingThrowId { get; init; }
}
