using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Platform.Domain;

namespace Starlights.Modules.Characters.Domain.Registrations;

/// <summary>
/// Represents a statistic rule that has been applied to a registration via a statistic rule component.
/// </summary>
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
    /// Factory for creating a new applied registration statistic rule.
    /// </summary>
    internal static RegistrationStatisticRule Create(RegistrationId parentRegistrationId, ElementComponentId associatedStatisticRuleId, string name, string value)
        => new(parentRegistrationId, associatedStatisticRuleId, name, value);



    public void UpdateStackingBonus(string stackingBonus) =>
        StackingBonus = stackingBonus.Trim();

    public void UpdateLevelRequirement(int levelRequirement)
        => LevelRequirement = levelRequirement;
}
