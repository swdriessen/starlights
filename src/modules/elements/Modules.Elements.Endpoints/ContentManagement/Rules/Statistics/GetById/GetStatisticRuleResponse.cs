namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetById;

public sealed record GetStatisticRuleResponse(
    Guid ElementId,
    Guid RuleId,
    string Name,
    string? DisplayName,
    string Value,
    string? StackingBonus,
    int LevelRequirement,
    string? Requirements,
    int? Minimum,
    int? Maximum,
    int OrderSequence);
