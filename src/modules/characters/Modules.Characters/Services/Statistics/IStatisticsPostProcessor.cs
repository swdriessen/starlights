namespace Starlights.Modules.Characters.Services.Statistics;

/// <summary>
/// Defines a post processor that runs after direct value rules are processed.
/// Post processors typically calculate derived statistics like proficiency variants.
/// </summary>
public interface IStatisticsPostProcessor
{
    /// <summary>
    /// Gets the execution order for this processor. Lower values execute first.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Processes statistics, adding or modifying values in the collection.
    /// </summary>
    /// <param name="context">The context containing all information needed for processing.</param>
    void Process(StatisticsProcessorContext context);
}
