using Starlights.Modules.Characters.Endpoints.Models;

namespace Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetSelectionRules;

public class GetSelectionRulesResponse
{
    public List<SelectionRuleDataModel> Rules { get; set; } = [];
}
