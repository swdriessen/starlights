namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.GetList;

public sealed record GetIncludeRulesResponse(Guid ElementId, IReadOnlyList<GetIncludeRulesResponse.IncludeRuleItem> Rules)
{
    public sealed record IncludeRuleItem(
        Guid RuleId,
        Guid IncludedElementId,
        int LevelRequirement,
        string? Requirements,
        string? DisplayName,
        int OrderSequence);
}
