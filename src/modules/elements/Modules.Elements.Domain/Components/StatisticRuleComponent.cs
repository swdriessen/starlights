using System.Diagnostics.CodeAnalysis;

namespace Starlights.Modules.Elements.Domain.Components;

public sealed class StatisticRuleComponent : ElementComponentBase
{
    public StatisticRuleComponent(ElementId owningElement, string name, string value, int levelRequirement)
        : base(owningElement)
    {
        UpdateName(name);
        UpdateValue(value);
        UpdateLevelRequirement(levelRequirement);
    }

    /// <summary>
    /// Gets or sets the name of the statistic.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets or sets the display name of the statistic.
    /// </summary>
    public string? DisplayName { get; private set; }

    /// <summary>
    /// Gets or sets the value of the statistic.
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Gets or sets the name of the stacking bonus of the statistic.
    /// </summary>
    public string? StackingBonus { get; private set; }

    /// <summary>
    /// Gets the level requirement for this statistic rule.
    /// </summary>
    public int LevelRequirement { get; private set; }

    /// <summary>
    /// Gets or sets a dynamic string that can be used to specify additional requirements for the selection option.
    /// </summary>
    public string? Requirements { get; private set; }

    /// <summary>
    /// Updates the name of the statistic. The value is normalized for statistic usage.
    /// </summary>
    [MemberNotNull(nameof(Name))]
    public void UpdateName(string name)
    {
        Name = name.NormalizeStatistic();
    }

    /// <summary>
    /// Updates the display name of the statistic.
    /// </summary>
    public void UpdateDisplayName(string? displayName)
    {
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
    }

    /// <summary>
    /// Updates the value of the statistic. The value is normalized for statistic usage.
    /// </summary>
    [MemberNotNull(nameof(Value))]
    public void UpdateValue(string value)
    {
        Value = value.IsNumeric() ? value.Trim(' ', '+') : value.NormalizeStatistic();
    }

    /// <summary>
    /// Updates the stacking bonus of the statistic. The value is normalized for statistic usage.
    /// </summary>
    public void UpdateStackingBonus(string value)
    {
        StackingBonus = value.NormalizeStatistic();
    }

    /// <summary>
    /// Checks if the value of the statistic is a number.
    /// </summary>
    public bool IsNumberValue()
    {
        return int.TryParse(Value, out _);
    }

    /// <summary>
    /// Gets the value of the statistic as an integer.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public int GetValue()
    {
        return int.TryParse(Value, out var result)
            ? result
            : throw new InvalidOperationException($"The value '{Value}' is not a valid integer.");
    }

    /// <summary>
    /// Updates the level requirement for this statistic rule.
    /// </summary>
    /// <param name="levelRequirement">The new level requirement.</param>
    public void UpdateLevelRequirement(int levelRequirement)
    {
        if (levelRequirement < 0)
        {
            throw new ArgumentException("LevelRequirement cannot be negative.", nameof(levelRequirement));
        }

        LevelRequirement = levelRequirement;
    }

    /// <summary>
    /// Gets a value indicating whether this selection option has any requirements.
    /// </summary>
    public bool HasRequirements => LevelRequirement > 0 || !string.IsNullOrWhiteSpace(Requirements);
}