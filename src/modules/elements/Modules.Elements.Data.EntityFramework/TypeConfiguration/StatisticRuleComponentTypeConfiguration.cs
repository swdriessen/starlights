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
            .IsRequired()
            .HasColumnName("name");

        builder.Property(x => x.Value)
            .IsRequired()
            .HasColumnName("value");

        builder.Property(x => x.LevelRequirement)
            .IsRequired()
            .HasColumnName("level_requirement");

        builder.Property(x => x.DisplayName)
            .HasColumnName("display_name");
        builder.Property(x => x.StackingBonus)
            .HasColumnName("stacking_bonus");
        builder.Property(x => x.Requirements)
            .HasColumnName("requirements");
    }
}