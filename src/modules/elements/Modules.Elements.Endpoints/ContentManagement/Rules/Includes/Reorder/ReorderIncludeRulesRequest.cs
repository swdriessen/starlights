namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Reorder;

public sealed record ReorderIncludeRulesRequest
{
    public Guid ElementId { get; init; }

    public required IReadOnlyList<Guid> RuleIds { get; init; }
}
