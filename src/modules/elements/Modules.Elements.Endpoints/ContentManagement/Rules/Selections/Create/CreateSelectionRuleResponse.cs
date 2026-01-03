using Starlights.Modules.Elements.Domain;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.Create;

public sealed record CreateSelectionRuleResponse(Guid ElementId, ElementComponentId RuleId);
