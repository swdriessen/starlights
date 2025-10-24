using Starlights.Modules.Characters.Domain.Progression;

namespace Starlights.Modules.Characters.Services.Statistics.Initializers;

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

        var levelGroup = context.Statistics.WithGroup("level", group =>
        {
            group.WithDisplayName("Level");
            group.WithValue(progression.CharacterLevel, "Character");
            group.Complete();
        });

        context.Statistics.WithGroupVariants(levelGroup.GroupName, levelGroup.DisplayName);
    }
}
