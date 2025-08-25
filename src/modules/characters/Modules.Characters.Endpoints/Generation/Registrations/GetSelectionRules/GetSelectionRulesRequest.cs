using FastEndpoints;

namespace Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetSelectionRules;

public class GetSelectionRulesRequest
{
    [BindFrom("type")]
    public string[] SelectionRuleTypes { get; set; } = [];
}
