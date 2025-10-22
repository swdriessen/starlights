using Starlights.Modules.Characters.Domain.Progression;

namespace Starlights.Modules.Characters.Services.Statistics.Processors;

/// <summary>
/// Initializes character-related statistics for a statistics processor context, including the character's level
/// information.
/// </summary>
/// <remarks>This initializer is intended for internal use within the statistics calculation pipeline. It
/// associates the character's progression data with the statistics context, enabling subsequent calculations to access
/// level-based statistics.</remarks>
internal sealed class CharacterStatisticsInitializer : IStatisticsCalculationInitializer
{
    public void Initialize(StatisticsProcessorContext context)
    {
        var progression = context.Character.GetRequiredComponent<ProgressionComponent>();

        context.Statistics.WithGroup("level", g =>
        {
            g.WithDisplayName("Level");
            g.WithValue(progression.CharacterLevel, "Character");
            g.Complete();
        });
        context.Statistics.WithGroupVariants("level", "Character");

        //context.Statistics.WithGroup("initiative", g => g.WithDisplayName("Initiative"));
        //context.Statistics.WithGroup("initiative:misc");

        context.Statistics.WithGroup("proficiency", g => g.WithDisplayName("Proficiency"));
        context.Statistics.WithGroup("proficiency:misc");
    }
}
