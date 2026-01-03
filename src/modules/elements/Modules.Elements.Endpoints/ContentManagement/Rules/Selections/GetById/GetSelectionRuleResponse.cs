namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.GetById;

public sealed record GetSelectionRuleResponse(
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
