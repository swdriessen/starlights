using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Classes.Eventing;

public sealed record CharacterClassRemovedEvent : CharacterEventBase
{
    public CharacterClassId ClassId { get; init; }
}