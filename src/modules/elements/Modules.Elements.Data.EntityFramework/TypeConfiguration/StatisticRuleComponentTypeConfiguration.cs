using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class StatisticRuleComponentTypeConfiguration : IEntityTypeConfiguration<StatisticRuleComponent>
{
    public void Configure(EntityTypeBuilder<StatisticRuleComponent> builder)
    {
        builder.ToTable("element_component_statistic_rule");

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired();

        builder.Property(x => x.LevelRequirement)
            .IsRequired();

        builder.Property(x => x.DisplayName);
        builder.Property(x => x.StackingBonus);
        builder.Property(x => x.Requirements);
    }
}