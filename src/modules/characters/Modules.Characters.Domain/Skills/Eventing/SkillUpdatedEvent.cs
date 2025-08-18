using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Skills.Eventing;

public record SkillUpdatedEvent : CharacterEventBase
{
    public SkillId SkillId { get; init; }
}