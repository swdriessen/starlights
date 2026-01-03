namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.Create;

public sealed record CreateSelectionRuleRequest(
    Guid ElementId,
    string DisplayName,
    string Type,
    string? Supports,
    string? Range,
    int Quantity,
    bool Optional,
    int LevelRequirement,
    string? Requirements
);
