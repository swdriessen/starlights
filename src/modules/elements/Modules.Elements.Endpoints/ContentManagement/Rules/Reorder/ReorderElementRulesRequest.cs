namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Reorder;

public sealed record ReorderElementRulesRequest
{

    public required IReadOnlyList<Guid> RuleIds { get; init; }
}
