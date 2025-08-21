namespace Starlights.Modules.Elements.Integration.Models.Rules;

/// <summary>
/// The DTO model for a statistic rule.
/// </summary>
public record StatisticRuleDataModel(Guid RuleId, string Name, string Value, string? StackingBonus, int LevelRequirement);
