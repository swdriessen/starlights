namespace Starlights.Modules.Elements.Integration.Models.Rules;

/// <summary>
/// The DTO model for an include rule.
/// </summary>
public record IncludeRuleDataModel(Guid RuleId, Guid IncludedElementId, int LevelRequirement);
