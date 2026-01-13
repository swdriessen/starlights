namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.GetById;

public sealed record GetIncludeRuleResponse(
    Guid ElementId,
    Guid RuleId,
    Guid IncludedElementId,
    int LevelRequirement,
    string? Requirements,
    string? DisplayName,
    int OrderSequence);
