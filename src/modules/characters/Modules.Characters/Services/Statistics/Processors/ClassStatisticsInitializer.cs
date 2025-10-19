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

        foreach (var c in component.Classes)
        {
            var slug = c.Name.ToLowerInvariant().Trim().Replace(' ', '-');
            var group = context.Statistics.WithGroup($"{slug}:level", g => g.WithInternalValue(c.Level));
            context.Statistics.WithGroupVariants(group.GroupName);
        }
    }
}