using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters.Eventing;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.SavingThrows;
using Starlights.Modules.Characters.Domain.SavingThrows.Eventing;
using Starlights.Modules.Characters.Domain.Skills;
using Starlights.Modules.Characters.Domain.Skills.Eventing;
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

    /// <summary>
    /// Updates the base score of an ability score for the character.
    /// </summary>
    /// <param name="abilityScoreId"></param>
    /// <param name="value"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void UpdateAbilityBaseScore(AbilityScoreId abilityScoreId, int value)
    {
        var ability = _abilityScores.SingleOrDefault(a => a.Id == abilityScoreId) ?? throw new InvalidOperationException($"AbilityScore with ID {abilityScoreId} not found for Character {Id}.");

        if (value is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Base score must be between 1 and 20.");
        }

        ability.UpdateBaseScore(value);
        AddDomainEvent(new AbilityScoreUpdatedEvent() { CharacterId = Id, AbilityScoreId = ability.Id });

        foreach (var skill in _skills)
        {
            if (skill.AbilityScoreId == abilityScoreId)
            {
                skill.UpdateAbilityScoreModifier(ability.CalculatedModifier);
                AddDomainEvent(new SkillUpdatedEvent { CharacterId = Id, SkillId = skill.Id });
            }
        }

        foreach (var savingThrow in _savingThrows)
        {
            if (savingThrow.AbilityScoreId == abilityScoreId)
            {
                savingThrow.UpdateAbilityScoreModifier(ability.CalculatedModifier);
                AddDomainEvent(new SavingThrowUpdatedEvent { CharacterId = Id, SavingThrowId = savingThrow.Id });
            }
        }
    }

    /// <summary>
    /// Updates the additional score of an ability score for the character.
    /// </summary>
    public void UpdateAbilityAdditionalScore(AbilityScoreId abilityScoreId, int value)
    {
        var ability = _abilityScores.SingleOrDefault(a => a.Id == abilityScoreId) ?? throw new InvalidOperationException($"AbilityScore with ID {abilityScoreId} not found for Character {Id}.");

        ability.UpdateAdditionalScore(value);
        AddDomainEvent(new AbilityScoreUpdatedEvent() { CharacterId = Id, AbilityScoreId = ability.Id });

        foreach (var skill in _skills)
        {
            if (skill.AbilityScoreId == abilityScoreId)
            {
                skill.UpdateAbilityScoreModifier(ability.CalculatedModifier);
                AddDomainEvent(new SkillUpdatedEvent { CharacterId = Id, SkillId = skill.Id });
            }
        }

        foreach (var savingThrow in _savingThrows)
        {
            if (savingThrow.AbilityScoreId == abilityScoreId)
            {
                savingThrow.UpdateAbilityScoreModifier(ability.CalculatedModifier);
                AddDomainEvent(new SavingThrowUpdatedEvent { CharacterId = Id, SavingThrowId = savingThrow.Id });
            }
        }
    }
}
