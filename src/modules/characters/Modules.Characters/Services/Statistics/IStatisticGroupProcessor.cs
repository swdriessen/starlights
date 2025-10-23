namespace Starlights.Modules.Characters.Services.Statistics;

/// <summary>
/// Defines a processor for handling specific statistic groups that require early or special processing.
/// </summary>
public interface IStatisticGroupProcessor
{
    /// <summary>
    /// Gets the execution order for this processor. Lower values execute first.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Processes the specified statistic group and removes it from pending groups if completed.
    /// </summary>
    /// <param name="pendingGroups">The collection of pending statistic rule groups.</param>
    /// <param name="context">The statistics processing context.</param>
    /// <param name="processGroupNode">Delegate to process a single group node.</param>
    void Process(Dictionary<string, StatisticRuleGroup> pendingGroups, StatisticsProcessorContext context, Func<StatisticRuleGroup, StatisticsProcessorContext, StatisticValuesGroup?> processGroupNode);
}
