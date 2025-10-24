using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Platform.Domain;
using Starlights.Platform.SourceGenerators.Entities.Attributes;

namespace Starlights.Modules.Characters.Domain.Registrations;

/// <summary>
/// Represents a statistic rule that has been applied to a registration via a statistic rule component.
/// </summary>
[Entity]
[DebuggerDisplay("Name = {Name}, Value = {Value}, Stack = {StackingBonus}, Level = {LevelRequirement}, Minimum = {MinimumValue}, Maximum = {MaximumValue}")]
public sealed class RegistrationStatisticRule : EntityBase<RegistrationStatisticRuleId>
{
    private RegistrationStatisticRule(RegistrationId parentRegistrationId, ElementComponentId associatedStatisticRuleId, string name, string value)
        : base(RegistrationStatisticRuleId.New())
    {
        ParentRegistrationId = parentRegistrationId;
        AssociatedStatisticRuleId = associatedStatisticRuleId;
        Name = name.Trim();
        Value = value.Trim();
    }

    /// <summary>
    /// Gets the parent registration associated with this statistic rule.
    /// </summary>
    public RegistrationId ParentRegistrationId { get; }

    /// <summary>
    /// Gets the ID of the statistic rule component that produced this applied rule.
    /// </summary>
    public ElementComponentId AssociatedStatisticRuleId { get; }

    /// <summary>
    /// Gets the normalized statistic name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the normalized statistic value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the optional stacking bonus value.
    /// </summary>
    public string? StackingBonus { get; private set; }

    /// <summary>
    /// Gets the level requirement at which this statistic rule becomes active.
    /// </summary>
    public int LevelRequirement { get; private set; }

    /// <summary>
    /// Gets the minimum value for the individual statistic rule, regardless of value provided based on assigned value, if one is defined.
    /// </summary>
    public int? MinimumValue { get; private set; }

    /// <summary>
    /// Gets the maximum allowable value for this individual statistic rule, if one is specified.
    /// </summary>
    public int? MaximumValue { get; private set; }

    /// <summary>
    /// Gets or sets a user-friendly name that identifies the statistic value. e.g. "Proficiency Bonus" instead of "proficiency".
    /// </summary>
    public string? FriendlyName { get; set; }

    /// <summary>
    /// Determines whether the current value is a reference value rather than a numeric value.
    /// </summary>
    /// <returns>true if the value is a reference value; otherwise, false.</returns>
    public bool HasReferenceValue()
    {
        return !int.TryParse(Value, out _);
    }

    /// <summary>
    /// Determines whether a stacking bonus is defined for the current instance.
    /// </summary>
    /// <returns>true if a stacking bonus is present and not empty; otherwise, false.</returns>
    public bool HasStackingBonus()
    {
        return !string.IsNullOrWhiteSpace(StackingBonus);
    }

    /// <summary>
    /// Parses the current value as a signed integer.
    /// </summary>
    /// <returns>The integer representation of the current value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the current value is not a valid number.</exception>
    public int GetValue()
    {
        if (int.TryParse(Value, out int value))
        {
            return value;
        }

        throw new FormatException($"The value '{Value}' is not a valid number.");
    }

    /// <summary>
    /// Updates the stacking bonus value after trimming any leading or trailing whitespace.
    /// </summary>
    /// <param name="stackingBonus">The new stacking bonus value to set. Can be null. Leading and trailing whitespace will be removed if not null.</param>
    public void UpdateStackingBonus(string? stackingBonus)
    {
        StackingBonus = stackingBonus?.Trim();
    }

    /// <summary>
    /// Updates the minimum level required to access the associated feature or resource.
    /// </summary>
    /// <param name="levelRequirement">The new minimum level required. Must be a non-negative integer.</param>
    public void UpdateLevelRequirement(int levelRequirement)
    {
        LevelRequirement = levelRequirement;
    }

    /// <summary>
    /// Updates the minimum value used by the current instance.
    /// </summary>
    /// <param name="minimumValue">The new minimum value to set. Specify null to remove the minimum value constraint.</param>
    public void UpdateMinimumValue(int? minimumValue)
    {
        MinimumValue = minimumValue;
    }

    /// <summary>
    /// Updates the maximum value used by the current instance.
    /// </summary>
    /// <param name="maximumValue">The new maximum value to set. Specify null to remove any maximum value constraint.</param>
    public void UpdateMaximumValue(int? maximumValue)
    {
        MaximumValue = maximumValue;
    }

    /// <summary>
    /// Factory for creating a new applied registration statistic rule.
    /// </summary>
    internal static RegistrationStatisticRule Create(RegistrationId parentRegistrationId, ElementComponentId associatedStatisticRuleId, string name, string value)
    {
        return new(parentRegistrationId, associatedStatisticRuleId, name, value);
    }


    // TODO: DisplayName =/ instead of parent name
}
