using Starlights.Modules.Characters.Domain.Abilities;

namespace Starlights.Modules.Characters.Services.Statistics.Processors;

/// <summary>
/// Initializes ability score and modifier statistics for a character within the statistics processing context.
/// </summary>
/// <remarks>This class is intended for internal use within the statistics calculation pipeline. It adds grouped
/// statistics for each ability score and its corresponding modifier, making them available for further processing or
/// display. This type is not intended to be used directly by application code.</remarks>
internal sealed class AbilitiesStatisticsInitializer : IStatisticsCalculationInitializer
{
    public void Initialize(StatisticsProcessorContext context)
    {
        var component = context.Character.GetRequiredComponent<AbilitiesComponent>();

        foreach (var score in component.AbilityScores)
        {
            var slug = score.Name.ToSlug();

            var scoreGroup = context.Statistics.WithGroup($"{slug}:score");
            var modifierGroup = context.Statistics.WithGroup($"{slug}:modifier");

            context.Statistics.WithGroupVariants(scoreGroup.GroupName, score.Name);
            context.Statistics.WithGroupVariants(modifierGroup.GroupName, score.Name);
        }
    }
}
