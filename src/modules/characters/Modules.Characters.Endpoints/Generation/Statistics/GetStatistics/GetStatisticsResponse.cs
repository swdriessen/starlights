using System.Diagnostics;

namespace Starlights.Modules.Characters.Endpoints.Generation.Statistics.GetStatistics;

/// <summary>
/// Represents a single statistic group with its calculated values.
/// </summary>
[DebuggerDisplay("GroupName = {GroupName}, TotalValue = {TotalValue}, IsFinalized = {IsFinalized}, ValuesCount = {Values.Count}")]
public sealed class StatisticGroupDataModel
{
    public required string GroupName { get; set; }
    public required int TotalValue { get; set; }
    public required List<StatisticValueDataModel> Values { get; set; }
    public required bool IsFinalized { get; set; }
}

/// <summary>
/// Represents a single value contribution to a statistic group.
/// </summary>
[DebuggerDisplay("Source = {Source}, Value = {Value}, DisplayName = {DisplayName}, RuleId = {RuleId}")]
public sealed class StatisticValueDataModel
{
    public required string Source { get; set; }
    public required int Value { get; set; }
    public string? DisplayName { get; set; }
    public Guid? RuleId { get; set; }
}

public sealed class GetStatisticsResponse
{
    public List<StatisticGroupDataModel> Statistics { get; set; } = [];
}
