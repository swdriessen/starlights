using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class SelectionRuleComponentTypeConfiguration : IEntityTypeConfiguration<SelectionRuleComponent>
{
    public void Configure(EntityTypeBuilder<SelectionRuleComponent> builder)
    {
        builder.ToTable("element_component_selection_rule");

        builder.Property(x => x.ElementType)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.LevelRequirement)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.IsOptional)
            .IsRequired();

        builder.Property(x => x.ShortDescription);
        builder.Property(x => x.Supports);
        builder.Property(x => x.RangeSupports);
        builder.Property(x => x.Requirements);
    }
}
