using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Skills;

public record SkillCreatedEvent : CharacterEventBase
{
    public SkillId SkillId { get; init; }
}
