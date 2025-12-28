namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.GetList;

public sealed class GetSelectionRulesResponse
{
    public List<SelectionRuleItem> Rules { get; set; } = [];

    public sealed record SelectionRuleItem(
        Guid RuleId,
        string DisplayName,
        string Type,
        string? Supports,
        string? Range,
        int Quantity,
        bool Optional,
        int LevelRequirement,
        string? Requirements
    );
}
