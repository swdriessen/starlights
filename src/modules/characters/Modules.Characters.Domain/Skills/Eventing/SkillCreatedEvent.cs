using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Skills.Eventing;

public record SkillCreatedEvent : CharacterEventBase
{
    public SkillId SkillId { get; init; }
}
