using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters.Eventing;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.Skills;
using Starlights.Modules.Characters.Domain.Skills.Eventing;
using Starlights.Modules.Characters.Domain.SavingThrows;
using Starlights.Modules.Characters.Domain.SavingThrows.Eventing;
using Starlights.Platform.Domain;

namespace Starlights.Modules.Characters.Domain.Characters;

/// <summary>
/// Represents a character in the system.
/// </summary>
[DebuggerDisplay("Id = {Id}, Name = {Name}")]
public sealed class Character : AggregateRoot<CharacterId>
{
    private readonly List<AbilityScore> _abilityScores = [];
    private readonly List<Skill> _skills = [];
    private readonly List<SavingThrow> _savingThrows = [];

    private Character(string name)
        : base(CharacterId.New())
    {
        Name = name;
    }

    /// <summary>
    /// Gets the name of the character.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the collection of ability scores associated with the character.
    /// </summary>
    public IReadOnlyCollection<AbilityScore> AbilityScores => _abilityScores.AsReadOnly();

    /// <summary>
    /// Gets the collection of skills associated with the character.
    /// </summary>
    public IReadOnlyCollection<Skill> Skills => _skills.AsReadOnly();

    /// <summary>
    /// Gets the collection of saving throws associated with the character.
    /// </summary>
    public IReadOnlyCollection<SavingThrow> SavingThrows => _savingThrows.AsReadOnly();

    /// <summary>
    /// Creates a new instance of the <see cref="Character"/> class with the specified name.
    /// </summary>
    public static Character Create(string name)
    {
        var newCharacter = new Character(name);
        newCharacter.AddDomainEvent(new CharacterCreatedEvent() { CharacterId = newCharacter.Id });
        return newCharacter;
    }

    /// <summary>
    /// Creates a new ability score for the character.
    /// </summary>
    public AbilityScore CreateAbilityScore(RegistrationId associatedRegistrationId, string name, string abbreviation)
    {
        var abilityScore = AbilityScore.Create(associatedRegistrationId, name, abbreviation);
        _abilityScores.Add(abilityScore);

        AddDomainEvent(new AbilityScoreCreatedEvent() { CharacterId = Id, AbilityScoreId = abilityScore.Id });

        return abilityScore;
    }

    /// <summary>
    /// Creates a new skill for the character.
    /// </summary>
    public Skill CreateSkill(RegistrationId associatedRegistrationId, string name, AbilityScoreId abilityScoreId, string abilityScoreAbbreviation)
    {
        var skill = Skill.Create(associatedRegistrationId, name, abilityScoreId, abilityScoreAbbreviation);
        _skills.Add(skill);

        AddDomainEvent(new SkillCreatedEvent() { CharacterId = Id, SkillId = skill.Id });

        return skill;
    }

    /// <summary>
    /// Creates a new skill for the character.
    /// </summary>
    public Skill CreateSkillWithoutAbilityScore(RegistrationId associatedRegistrationId, string name)
    {
        var skill = Skill.CreateWithoutAbilityScore(associatedRegistrationId, name);
        _skills.Add(skill);

        AddDomainEvent(new SkillCreatedEvent() { CharacterId = Id, SkillId = skill.Id });

        return skill;
    }

    /// <summary>
    /// Creates a new saving throw for the character.
    /// </summary>
    public SavingThrow CreateSavingThrow(RegistrationId associatedRegistrationId, string name, AbilityScoreId abilityScoreId, string abilityScoreAbbreviation)
    {
        var save = SavingThrow.Create(associatedRegistrationId, name, abilityScoreId, abilityScoreAbbreviation);
        _savingThrows.Add(save);
        AddDomainEvent(new SavingThrowCreatedEvent { CharacterId = Id, SavingThrowId = save.Id });
        return save;
    }

    /// <summary>
    /// Creates a new saving throw for the character without an associated ability score (should be rare).
    /// </summary>
    public SavingThrow CreateSavingThrowWithoutAbilityScore(RegistrationId associatedRegistrationId, string name)
    {
        var save = SavingThrow.CreateWithoutAbilityScore(associatedRegistrationId, name);
        _savingThrows.Add(save);
        AddDomainEvent(new SavingThrowCreatedEvent { CharacterId = Id, SavingThrowId = save.Id });
        return save;
    }
}
