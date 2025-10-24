using Starlights.Modules.Characters.Domain.Classes;

namespace Starlights.Modules.Characters.Services.Statistics.Initializers;

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
        context.Character.UpdateComponent<ClassComponent>((component, _) =>
        {
            foreach (var classObj in component.Classes)
            {
                var slug = classObj.Name.ToSlug();

                var levelGroup = context.Statistics.WithGroup($"{slug}:level", group =>
                {
                    group.WithValue(classObj.Level, classObj.Name);
                    group.Complete();
                });

                context.Statistics.WithGroupVariants(levelGroup.GroupName, classObj.Name);
            }
        });
    }
}