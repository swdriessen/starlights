using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.SavingThrows.Eventing;

public sealed record SavingThrowUpdatedEvent : CharacterEventBase
{
    public SavingThrowId SavingThrowId { get; init; }
}