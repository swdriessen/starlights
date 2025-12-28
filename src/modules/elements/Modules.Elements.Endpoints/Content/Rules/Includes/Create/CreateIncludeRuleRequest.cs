using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Create;

public sealed record CreateIncludeRuleRequest(
    Guid ElementId,
    Guid IncludedElementId,
    int LevelRequirement,
    string? RequirementsExpression,
    string? DisplayName
);
