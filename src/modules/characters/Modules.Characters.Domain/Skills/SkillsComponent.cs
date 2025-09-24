using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Components;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.Skills.Eventing;

namespace Starlights.Modules.Characters.Domain.Skills;

public sealed class SkillsComponent : CharacterComponentBase
{
    private readonly List<Skill> _skills = [];

    private SkillsComponent(CharacterId parentCharacter)
        : base(parentCharacter)
    {
    }

    /// <summary>
    /// Gets the collection of skills associated with the parent character.
    /// </summary>
    public IReadOnlyCollection<Skill> Skills => _skills.AsReadOnly();

    /// <summary>
    /// Creates a new skill associated with the specified registration and ability score.
    /// </summary>
    public Skill CreateSkill(RegistrationId associatedRegistrationId, string name, AbilityScoreId abilityScoreId, string abilityScoreAbbreviation)
    {
        var skill = Skill.Create(associatedRegistrationId, name, abilityScoreId, abilityScoreAbbreviation);
        _skills.Add(skill);
        AddDomainEvent(new SkillCreatedEvent() { CharacterId = Id, SkillId = skill.Id });
        return skill;
    }

    /// <summary>
    /// Updates the ability score modifier for all skills associated with the specified ability score.
    /// </summary>
    public void UpdateAbilityScoreModifier(AbilityScoreId abilityScoreId, int modifier)
    {
        foreach (var skill in _skills.Where(s => s.AbilityScoreId == abilityScoreId))
        {
            if (skill.UpdateAbilityScoreModifier(modifier))
            {
                AddDomainEvent(new SkillUpdatedEvent() { CharacterId = ParentCharacter, SkillId = skill.Id });
            }
        }
    }

    /// <summary>
    /// Creates a new instance of the SkillsComponent class for the specified character.
    /// </summary>
    public static SkillsComponent Create(CharacterId parentCharacter)
    {
        return new SkillsComponent(parentCharacter);
    }
}
