namespace Starlights.Modules.Elements.Integration.Models;

/// <summary>
/// The DTO model for an include rule.
/// </summary>
public record IncludeRuleDataModel(Guid RuleId, Guid IncludedElementId, int LevelRequirement);
