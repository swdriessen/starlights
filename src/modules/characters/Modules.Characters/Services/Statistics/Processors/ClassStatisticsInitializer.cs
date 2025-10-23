using Starlights.Modules.Characters.Domain.Classes;

namespace Starlights.Modules.Characters.Services.Statistics.Processors;

/// <summary>
/// Initializes class-based statistics for a character within the statistics processing context.
/// </summary>
/// <remarks>This type is intended for internal use within the statistics calculation pipeline. It associates each
/// character class with a statistics group based on the class name and level. This enables downstream components to
/// access class-specific statistics during processing.</remarks>
internal sealed class ClassStatisticsInitializer : IStatisticsCalculationInitializer
{
    public void Initialize(StatisticsProcessorContext context)
    {
        var component = context.Character.GetRequiredComponent<ClassComponent>();

        foreach (var characterClass in component.Classes)
        {
            var slug = characterClass.Name.ToSlug();

            var levelGroup = context.Statistics.WithGroup($"{slug}:level", group =>
            {
                group.WithValue(characterClass.Level, characterClass.Name);
                group.Complete();
            });

            context.Statistics.WithGroupVariants(levelGroup.GroupName, characterClass.Name);
        }
    }
}