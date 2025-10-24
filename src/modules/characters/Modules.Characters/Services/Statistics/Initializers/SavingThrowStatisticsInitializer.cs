using Starlights.Modules.Characters.Domain.SavingThrows;

namespace Starlights.Modules.Characters.Services.Statistics.Initializers;

internal sealed class SavingThrowStatisticsInitializer : IStatisticsCalculationInitializer
{
    public void Initialize(StatisticsProcessorContext context)
    {
        var component = context.Character.GetRequiredComponent<SavingThrowsComponent>();

        foreach (var save in component.SavingThrows)
        {
            var slug = save.Name.ToSlug();

            context.Statistics.WithGroup($"{slug}:proficiency");
            context.Statistics.WithGroup($"{slug}:misc");
        }
    }
}
