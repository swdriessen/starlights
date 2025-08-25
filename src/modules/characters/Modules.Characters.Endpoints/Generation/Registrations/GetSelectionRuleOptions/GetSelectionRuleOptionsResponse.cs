using Starlights.Modules.Characters.Endpoints.Models;

namespace Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetSelectionRuleOptions;

public class GetSelectionRuleOptionsResponse
{
    public List<SelectionRuleOptionModel> Options { get; set; } = [];
}