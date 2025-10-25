namespace Starlights.Modules.Characters.Services.Statistics;

/// <summary>
/// Defines a seed processor that runs before direct value rules are processed.
/// Seed processors typically initialize base statistics like character level and ability scores.
/// </summary>
public interface IStatisticsCalculationInitializer
{
    /// <summary>
    /// Processes statistics, adding or modifying values in the collection.
    /// </summary>
    /// <param name="context">The context containing all information needed for processing.</param>
    void Initialize(StatisticsProcessorContext context);
}
