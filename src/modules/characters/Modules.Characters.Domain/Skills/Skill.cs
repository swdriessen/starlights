using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Domain;
using Starlights.Platform.SourceGenerators.Entities.Attributes;

namespace Starlights.Modules.Characters.Domain.Skills;

[Entity]
public sealed class Skill : EntityBase<SkillId>
{
    private Skill(RegistrationId associatedRegistrationId, string name, AbilityScoreId abilityScoreId, string? abilityScoreAbbreviation)
        : base(SkillId.New())
    {
        AssociatedRegistrationId = associatedRegistrationId;
        Name = name;
        AbilityScoreId = abilityScoreId;
        AbilityScoreAbbreviation = abilityScoreAbbreviation;
    }

    /// <summary>
    /// The registration this ability score is associated with.
    /// </summary>
    public RegistrationId AssociatedRegistrationId { get; }

    /// <summary>
    /// The ability score this skill is associated with (e.g. Strength for Athletics).
    /// </summary>
    public AbilityScoreId AbilityScoreId { get; private set; }

    /// <summary>
    /// The display name of the ability (e.g. Strength).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The abbreviated name of the ability score associated with the skill (e.g. STR).
    /// </summary>
    public string? AbilityScoreAbbreviation { get; private set; }

    /// <summary>
    /// The value of the ability score modifier associated with this skill.
    /// </summary>
    public int AbilityScoreModifier { get; private set; }

    /// <summary>
    /// The value of the additional bonus or penalty applied to the skill.
    /// </summary>
    public int AdditionalBonus { get; private set; }

    /// <summary>
    /// The calculated bonus for this skill, derived from the associated ability score.
    /// </summary>
    public int CalculatedBonus { get; private set; }

    /// <summary>
    /// Recalculates the derived bonus.
    /// </summary>
    private void Recalculate()
    {
        var value = AbilityScoreModifier + AdditionalBonus;

        if (CalculatedBonus == value)
        {
            return;
        }

        CalculatedBonus = value;
    }

    /// <summary>
    /// Updates the ability score modifier if the specified value differs from the current modifier.
    /// </summary>
    /// <returns>true if the ability score modifier was updated; otherwise, false.</returns>
    public bool UpdateAbilityScoreModifier(int modifier)
    {
        if (AbilityScoreModifier == modifier)
        {
            return false;
        }

        AbilityScoreModifier = modifier;
        Recalculate();
        return true;
    }

    /// <summary>
    /// Updates the value of the additional bonus if it differs from the current value.
    /// </summary>
    /// <returns>true if the additional bonus was updated; otherwise, false.</returns>
    public bool UpdateAdditionalBonus(int bonus)
    {
        if (AdditionalBonus == bonus)
        {
            return false;
        }

        AdditionalBonus = bonus;
        Recalculate();
        return true;
    }

    /// <summary>
    /// Associates this skill with an ability score.
    /// </summary>
    public void WithAbilityScore(AbilityScoreId abilityScoreId, string abilityScoreAbbreviation)
    {
        if (abilityScoreId == default)
        {
            throw new ArgumentException("Ability score ID cannot be default.", nameof(abilityScoreId));
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(abilityScoreAbbreviation);

        AbilityScoreId = abilityScoreId;
        AbilityScoreAbbreviation = abilityScoreAbbreviation;
    }

    /// <summary>
    /// Get a value indicating whether this skill has an associated ability score.
    /// </summary>
    public bool HasAssociatedAbilityScore => AbilityScoreId != default;

    /// <summary>
    /// Creates a new Skill instance with the specified registration, name, ability score identifier, and ability score abbreviation.
    /// </summary>
    internal static Skill Create(RegistrationId associatedRegistrationId, string name, AbilityScoreId abilityScoreId, string abilityScoreAbbreviation)
    {
        return new(associatedRegistrationId, name, abilityScoreId, abilityScoreAbbreviation);
    }
}
