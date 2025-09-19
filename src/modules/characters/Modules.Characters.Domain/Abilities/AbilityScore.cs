using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Domain;
using Starlights.Platform.SourceGenerators.Entities.Attributes;

namespace Starlights.Modules.Characters.Domain.Abilities;

[Entity]
public sealed class AbilityScore : EntityBase<AbilityScoreId>
{
    private AbilityScore(RegistrationId associatedRegistrationId, string name, string abbreviation)
        : base(AbilityScoreId.New())
    {
        AssociatedRegistrationId = associatedRegistrationId;
        Name = name;
        Abbreviation = abbreviation;
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
    /// Additional bonus or penalty applied to the base score. Defaults to 0.
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
    /// Updates the <see cref="BaseScore"/> and recalculates derived values.
    /// </summary>
    public void UpdateBaseScore(int value)
    {
        BaseScore = value;
        Recalculate();
    }

    /// <summary>
    /// Updates the <see cref="AdditionalScore"/> and recalculates derived values.
    /// </summary>
    public void UpdateAdditionalScore(int value)
    {
        AdditionalScore = value;
        Recalculate();
    }

    /// <summary>
    /// Recomputes <see cref="CalculatedScore"/> and <see cref="CalculatedModifier"/> from current inputs.
    /// </summary>
    private void Recalculate()
    {
        CalculatedScore = BaseScore + AdditionalScore;
        CalculatedModifier = (int)Math.Floor((CalculatedScore - 10) / 2.0);
    }

    /// <summary>
    /// Factory method to create a new <see cref="AbilityScore"/>.
    /// </summary>
    internal static AbilityScore Create(RegistrationId associatedRegistrationId, string name, string abbreviation)
    {
        return new AbilityScore(associatedRegistrationId, name, abbreviation);
    }
}
