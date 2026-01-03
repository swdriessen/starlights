namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetList;

public sealed record GetStatisticRulesResponse(Guid ElementId, IReadOnlyList<GetStatisticRulesResponse.StatisticRuleItem> Rules)
{
    public sealed record StatisticRuleItem(
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
}
