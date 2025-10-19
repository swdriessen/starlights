namespace Starlights.Modules.Characters.Services.Statistics.Processors;

/// <summary>
/// Calculates proficiency bonus variants (half proficiency, etc.).
/// </summary>
internal sealed class ProficiencyVariantsCalculator : IStatisticsPostProcessor
{
    public void Process(StatisticsProcessorContext context)
    {
        if (!context.Statistics.ContainsGroup("proficiency"))
        {
            return;
        }

        context.Statistics.WithGroupVariants("proficiency");
    }
}
