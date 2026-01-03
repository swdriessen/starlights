namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.Update;

public sealed record UpdateSelectionRuleRequest(
    string DisplayName,
    string Type,
    string? Supports,
    string? Range,
    int Quantity,
    bool Optional,
    int LevelRequirement,
    string? Requirements
);
