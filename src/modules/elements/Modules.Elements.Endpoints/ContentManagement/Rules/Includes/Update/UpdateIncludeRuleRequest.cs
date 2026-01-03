namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Update;

public sealed record UpdateIncludeRuleRequest
{
    public Guid ElementId { get; init; }
    public Guid RuleId { get; init; }

    public required Guid IncludedElementId { get; init; }
    public int LevelRequirement { get; init; }
    public string? RequirementsExpression { get; init; }
    public string? DisplayName { get; init; }
}
