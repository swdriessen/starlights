namespace Starlights.Modules.Characters.Services.Statistics;

public sealed class StatisticsCalculationResult
{
    public StatisticsCalculationResult(StatisticValuesGroupCollection statistics, IEnumerable<string>? errors = null)
    {
        Statistics = statistics;
        if (errors is not null)
        {
            Errors.AddRange(errors);
        }
    }

    /// <summary>
    /// Gets the collection of completed statistic values.
    /// </summary>
    public StatisticValuesGroupCollection Statistics { get; }

    /// <summary>
    /// Gets the list of error messages associated with the current calculation.
    /// </summary>
    public List<string> Errors { get; } = [];

    /// <summary>
    /// Gets a value indicating whether any errors are present.
    /// </summary>
    public bool HasErrors => Errors.Count > 0;
}