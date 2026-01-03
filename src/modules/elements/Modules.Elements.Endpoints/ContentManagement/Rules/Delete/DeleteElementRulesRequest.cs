namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Delete;

public sealed record DeleteElementRulesRequest(Guid ElementId, IReadOnlyList<Guid> RuleIds);
