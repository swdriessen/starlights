using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Domain;
using Starlights.Platform.SourceGenerators.Entities.Attributes;

namespace Starlights.Modules.Characters.Domain.Abilities;

[Entity]
public sealed class AbilityScore : EntityBase<AbilityScoreId>
{
    private AbilityScore(RegistrationId associatedRegistrationId, string name, string abbreviation, double sortingOrder = 0)
        : base(AbilityScoreId.New())
    {
        AssociatedRegistrationId = associatedRegistrationId;
        Name = name;
        Abbreviation = abbreviation;
        SortingOrder = sortingOrder;
        Recalculate();
    }

    /// <summary>
    /// The registration this ability score is associated with.
    /// </summary>
    public RegistrationId AssociatedRegistrationId { get; }

    /// <summary>
    /// The display name of the ability (e.g. Strength).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The abbreviated name of the ability (e.g. STR).
    /// </summary>
    public string Abbreviation { get; }

    /// <summary>
    /// The base score for the ability. Defaults to 10.
    /// </summary>
    public int BaseScore { get; private set; } = 10;

    /// <summary>
    /// Additional bonus or penalty applied to the base score.
    /// </summary>
    public int AdditionalScore { get; private set; }

    /// <summary>
    /// Derived total: BaseScore + AdditionalScore. Persisted and updated when inputs change.
    /// </summary>
    public int CalculatedScore { get; private set; }

    /// <summary>
    /// Gets the ability modifier computed from <see cref="CalculatedScore"/> as floor((score - 10) / 2).
    /// Persisted and updated when <see cref="CalculatedScore"/> changes.
    /// </summary>
    public int CalculatedModifier { get; private set; }

    /// <summary>
    /// Gets the sorting order value used to determine the relative position of this item in sorted collections.
    /// </summary>
    public double SortingOrder { get; internal set; }

    /// <summary>
    /// Updates the base score to the specified value if it differs from the current value.
    /// </summary>
    /// <returns>true if the base score was updated; otherwise, false.</returns>
    internal bool UpdateBaseScore(int value)
    {
        if (BaseScore == value)
        {
            return false;
        }

        BaseScore = value;
        Recalculate();
        return true;
    }

    /// <summary>
    /// Updates the <see cref="AdditionalScore"/> and recalculates derived values.
    /// Returns true if the value changed (and recalculation occurred); otherwise false.
    /// </summary>
    internal bool UpdateAdditionalScore(int value)
    {
        if (AdditionalScore == value)
        {
            return false;
        }

        AdditionalScore = value;
        Recalculate();
        return true;
    }

    /// <summary>
    /// Recalculates the calculated score and modifier based on the current base and additional scores.
    /// </summary>
    private void Recalculate()
    {
        CalculatedScore = BaseScore + AdditionalScore;
        CalculatedModifier = (int)Math.Floor((CalculatedScore - 10) / 2.0);
    }

    /// <summary>
    /// Creates a new instance of the AbilityScore class with the specified registration identifier, name, and abbreviation.
    /// </summary>
    internal static AbilityScore Create(RegistrationId associatedRegistrationId, string name, string abbreviation, double sortingOrder = 0)
    {
        return new AbilityScore(associatedRegistrationId, name, abbreviation, sortingOrder);
    }
}
