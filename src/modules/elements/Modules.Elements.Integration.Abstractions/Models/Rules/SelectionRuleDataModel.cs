namespace Starlights.Modules.Elements.Integration.Models.Rules;

/// <summary>
/// The DTO model for a selection rule.
/// </summary>
public record SelectionRuleDataModel(Guid RuleId, string ElementType, string Name, int LevelRequirement);