using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Domain;

namespace Starlights.Modules.Characters.Domain.Skills;

public class Skill : EntityBase<SkillId>
{
    public Skill(RegistrationId associatedRegistrationId, string name, AbilityScoreId abilityScoreId, string? abilityScoreAbbreviation)
        : base(SkillId.New())
    {
        AssociatedRegistrationId = associatedRegistrationId;
        Name = name;
        AbilityScoreId = abilityScoreId;
        AbilityScoreAbbreviation = abilityScoreAbbreviation;
        Recalculate();
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
        CalculatedBonus = AbilityScoreModifier + AdditionalBonus;
    }

    /// <summary>
    /// Updates the ability score modifier for this skill and recalculates the total bonus.
    /// </summary>
    /// <param name="modifier">The modifier value derived from the associated ability score.</param>
    public void UpdateAbilityScoreModifier(int modifier)
    {
        AbilityScoreModifier = modifier;
        Recalculate();
    }

    /// <summary>
    /// Updates the additional bonus (or penalty) for this skill and recalculates the total bonus.
    /// </summary>
    /// <param name="bonus">The additional bonus; can be negative to represent a penalty.</param>
    public void UpdateAdditionalBonus(int bonus)
    {
        AdditionalBonus = bonus;
        Recalculate();
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
    /// Factory method to create a new <see cref="Skill"/>.
    /// </summary>
    internal static Skill Create(RegistrationId associatedRegistrationId, string name, AbilityScoreId abilityScoreId, string abilityScoreAbbreviation)
    {
        return new Skill(associatedRegistrationId, name, abilityScoreId, abilityScoreAbbreviation);
    }

    /// <summary>
    /// Factory method to create a new <see cref="Skill"/>.
    /// </summary>
    internal static Skill CreateWithoutAbilityScore(RegistrationId associatedRegistrationId, string name)
    {
        return new Skill(associatedRegistrationId, name, default, default);
    }
}
