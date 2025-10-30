using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Domain;
using Starlights.Platform.SourceGenerators.Entities.Attributes;

namespace Starlights.Modules.Characters.Domain.SavingThrows;

/// <summary>
/// Represents a saving throw for a character (e.g. Strength Saving Throw) which is associated with a primary ability.
/// </summary>
[Entity]
public sealed class SavingThrow : EntityBase<SavingThrowId>
{
    public SavingThrow(RegistrationId associatedRegistrationId, string name, AbilityScoreId abilityScoreId, string? abilityScoreAbbreviation, double sortingOrder = 0)
        : base(SavingThrowId.New())
    {
        AssociatedRegistrationId = associatedRegistrationId;
        Name = name;
        AbilityScoreId = abilityScoreId;
        AbilityScoreAbbreviation = abilityScoreAbbreviation;
        SortingOrder = sortingOrder;
        Recalculate();
    }

    /// <summary>
    /// Gets the registration identifier associated with this instance.
    /// </summary>
    public RegistrationId AssociatedRegistrationId { get; }

    /// <summary>
    /// Gets the name of the saving throw.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the ability score identifier associated with this saving throw.
    /// </summary>
    public AbilityScoreId AbilityScoreId { get; }

    /// <summary>
    /// Gets the abbreviated form of the ability score name, such as "STR" for Strength.
    /// </summary>
    public string? AbilityScoreAbbreviation { get; }

    /// <summary>
    /// Gets the modifier value derived from the associated ability score.
    /// </summary>
    public int AbilityScoreModifier { get; private set; }

    /// <summary>
    /// Gets the additional bonus value applied to the calculation or operation.
    /// </summary>
    public int AdditionalBonus { get; private set; }

    /// <summary>
    /// Gets the calculated bonus value for the current instance.
    /// </summary>
    public int CalculatedBonus { get; private set; }

    /// <summary>
    /// Gets the sorting order value used to determine the relative position of this item in sorted collections.
    /// </summary>
    public double SortingOrder { get; internal set; }

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
    /// Updates the additional bonus value if it differs from the current value.
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
    /// Recalculates the value of the calculated bonus based on the current ability score modifier and any additional bonus.
    /// </summary>
    private void Recalculate()
    {
        CalculatedBonus = AbilityScoreModifier + AdditionalBonus;
    }

    /// <summary>
    /// Gets a value indicating whether this instance has an associated ability score.
    /// </summary>
    public bool HasAssociatedAbilityScore => AbilityScoreId != default;

    /// <summary>
    /// Creates a new instance of the SavingThrow class with the specified registration and ability score information.
    /// </summary>
    internal static SavingThrow Create(RegistrationId associatedRegistrationId, string name, AbilityScoreId abilityScoreId, string abilityScoreAbbreviation, double sortingOrder = 0)
    {
        return new(associatedRegistrationId, name, abilityScoreId, abilityScoreAbbreviation, sortingOrder);
    }
}
