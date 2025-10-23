namespace Starlights.Modules.Characters.Services.Statistics;

public sealed class StatisticCalculationResult
{
    public StatisticCalculationResult(StatisticValuesGroupCollection statistics, IEnumerable<string>? errors = null)
    {
        Statistics = statistics;
        if (errors is not null)
        {
            Errors.AddRange(errors);
        }
    }

    public StatisticValuesGroupCollection Statistics { get; }
    public bool HasErrors => Errors.Count > 0;
    public List<string> Errors { get; } = [];
}