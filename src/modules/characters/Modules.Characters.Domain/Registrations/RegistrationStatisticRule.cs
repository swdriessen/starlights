using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Platform.Domain;
using Starlights.Platform.SourceGenerators.Entities.Attributes;

namespace Starlights.Modules.Characters.Domain.Registrations;

/// <summary>
/// Represents a statistic rule that has been applied to a registration via a statistic rule component.
/// </summary>
[Entity]
[DebuggerDisplay("Id = {Id}, Parent = {ParentRegistrationId} RuleId = {AssociatedStatisticRuleId}, Name = {Name}, Value = {Value}")]
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
    /// Determines whether the <see cref="Value"/> represents a valid integer, optionally prefixed with a plus or minus sign.
    /// </summary>
    /// <returns>true if the value is a valid integer; otherwise, false.</returns>
    public bool IsNumberValue()
    {
        if (Value.StartsWith('+') || Value.StartsWith('-'))
        {
            return int.TryParse(Value[1..], out _);
        }

        return int.TryParse(Value, out _);
    }

    public bool HasReferenceValue()
    {
        return !IsNumberValue();
    }
    public bool HasStackingBonus()
    {
        return !string.IsNullOrWhiteSpace(StackingBonus);
    }

    /// <summary>
    /// Retrieves the integer value represented by the current object if it contains a valid number.
    /// </summary>
    /// <remarks>If the value starts with a '+' or '-' sign, the sign is ignored and only the numeric portion
    /// is parsed. This method does not support parsing negative numbers; only the sign character is skipped.</remarks>
    public int GetValue()
    {
        if (IsNumberValue())
        {
            if (Value.StartsWith('+'))
            {
                return int.Parse(Value[1..]);
            }
            return int.Parse(Value);
        }

        throw new InvalidOperationException($"The value '{Value}' is not a valid number.");

    }

    /// <summary>
    /// Factory for creating a new applied registration statistic rule.
    /// </summary>
    internal static RegistrationStatisticRule Create(RegistrationId parentRegistrationId, ElementComponentId associatedStatisticRuleId, string name, string value)
    {
        return new(parentRegistrationId, associatedStatisticRuleId, name, value);
    }

    public void UpdateStackingBonus(string stackingBonus)
    {
        StackingBonus = stackingBonus.Trim();
    }

    public void UpdateLevelRequirement(int levelRequirement)
    {
        LevelRequirement = levelRequirement;
    }

    public void UpdateMinimumValue(int? minimumValue)
    {
        MinimumValue = minimumValue;
    }

    public void UpdateMaximumValue(int? maximumValue)
    {
        MaximumValue = maximumValue;
    }


    // TODO: DisplayName =/ instead of parent name
}
