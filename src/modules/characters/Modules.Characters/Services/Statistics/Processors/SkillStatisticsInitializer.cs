using Starlights.Modules.Characters.Domain.Skills;

namespace Starlights.Modules.Characters.Services.Statistics.Processors;

internal sealed class SkillStatisticsInitializer : IStatisticsCalculationInitializer
{
    public void Initialize(StatisticsProcessorContext context)
    {
        var component = context.Character.GetRequiredComponent<SkillsComponent>();

        foreach (var save in component.Skills)
        {
            var slug = save.Name.ToLowerInvariant().Replace(' ', '_');

            context.Statistics.WithGroup($"{slug}", g => g.WithValue(save.CalculatedBonus, save.Name));
            context.Statistics.WithGroup($"{slug}:proficiency");
            context.Statistics.WithGroup($"{slug}:misc");
            context.Statistics.WithGroup($"{slug}:passive");
        }
    }
}
