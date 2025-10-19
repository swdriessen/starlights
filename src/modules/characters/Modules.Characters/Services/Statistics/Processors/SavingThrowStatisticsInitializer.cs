using Starlights.Modules.Characters.Domain.SavingThrows;

namespace Starlights.Modules.Characters.Services.Statistics.Processors;

internal sealed class SavingThrowStatisticsInitializer : IStatisticsCalculationInitializer
{
    public void Initialize(StatisticsProcessorContext context)
    {
        var component = context.Character.GetRequiredComponent<SavingThrowsComponent>();

        foreach (var save in component.SavingThrows)
        {
            var slug = save.Name.Replace("Saving Throw", "").Trim().ToLowerInvariant().Replace(' ', '_');

            context.Statistics.WithGroup($"{slug}:save", g => g.WithValue(save.CalculatedBonus, save.Name));
            context.Statistics.WithGroup($"{slug}:save:proficiency");
            context.Statistics.WithGroup($"{slug}:save:misc");
        }
    }
}
