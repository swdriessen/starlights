using Starlights.Modules.Characters.Domain.SavingThrows;

namespace Starlights.Modules.Characters.Services.Statistics.Processors;

internal sealed class SavingThrowStatisticsPostProcessor : IStatisticsPostProcessor
{
    public int Order => 10; // after proficiency and abilities

    public void Process(StatisticsProcessorContext context)
    {
        context.Character.UpdateComponent<SavingThrowsComponent>((component, _) =>
        {
            foreach (var save in component.SavingThrows)
            {
                // we can't just use "strength" here because to would clash with abilities, so we use full "strength-saving-throw" for now
                var slug = $"{save.Name.ToSlug()}";

                var additionalBonus = context.Statistics.GetGroupSum($"{slug}:proficiency");
                additionalBonus += context.Statistics.GetGroupSum($"{slug}:misc");

                save.UpdateAdditionalBonus(additionalBonus);
            }
        });
    }
}