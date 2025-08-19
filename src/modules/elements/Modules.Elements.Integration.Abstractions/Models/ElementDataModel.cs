namespace Starlights.Modules.Elements.Integration.Models;

/// <summary>
/// The DTO model for an Element with its associated data.
/// </summary>
public record ElementDataModel
{
    public Guid Id { get; init; } = Guid.Empty;
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;

    public List<IncludeRuleDataModel> IncludeRules { get; init; } = [];
    public List<StatisticRuleDataModel> StatisticRules { get; init; } = [];
}
