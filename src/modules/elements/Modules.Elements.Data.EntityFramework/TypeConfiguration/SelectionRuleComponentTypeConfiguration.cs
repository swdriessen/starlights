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
            .IsRequired()
            .HasColumnName("element_type");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnName("name");

        builder.Property(x => x.LevelRequirement)
            .IsRequired()
            .HasColumnName("level_requirement");

        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasColumnName("quantity");

        builder.Property(x => x.IsOptional)
            .IsRequired()
            .HasColumnName("is_optional");

        builder.Property(x => x.ShortDescription)
            .HasColumnName("short_description");

        builder.Property(x => x.Supports)
            .HasColumnName("supports");

        builder.Property(x => x.RangeSupports)
            .HasColumnName("range_supports");

        builder.Property(x => x.Requirements)
            .HasColumnName("requirements");
    }
}
