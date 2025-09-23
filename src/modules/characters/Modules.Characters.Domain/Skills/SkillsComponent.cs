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

    public IReadOnlyCollection<Skill> Skills => _skills.AsReadOnly();

    public Skill CreateSkill(RegistrationId associatedRegistrationId, string name, AbilityScoreId abilityScoreId, string abilityScoreAbbreviation)
    {
        var skill = Skill.Create(associatedRegistrationId, name, abilityScoreId, abilityScoreAbbreviation);
        _skills.Add(skill);
        AddDomainEvent(new SkillCreatedEvent() { CharacterId = Id, SkillId = skill.Id });
        return skill;
    }

    public Skill CreateSkillWithoutAbilityScore(RegistrationId associatedRegistrationId, string name)
    {
        var skill = Skill.CreateWithoutAbilityScore(associatedRegistrationId, name);
        _skills.Add(skill);
        AddDomainEvent(new SkillCreatedEvent() { CharacterId = Id, SkillId = skill.Id });
        return skill;
    }





    public void UpdateAbilityScoreModifier(AbilityScoreId abilityScoreId, int modifier)
    {
        foreach (var skill in _skills.Where(s => s.AbilityScoreId == abilityScoreId))
        {
            skill.UpdateAbilityScoreModifier(modifier);
            AddDomainEvent(new SkillUpdatedEvent() { CharacterId = ParentCharacter, SkillId = skill.Id });
        }
    }














    public static SkillsComponent Create(CharacterId parentCharacter)
    {
        return new SkillsComponent(parentCharacter);
    }
}
