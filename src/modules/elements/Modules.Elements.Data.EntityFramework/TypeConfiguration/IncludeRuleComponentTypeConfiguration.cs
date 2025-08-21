using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class IncludeRuleComponentTypeConfiguration : IEntityTypeConfiguration<IncludeRuleComponent>
{
    public void Configure(EntityTypeBuilder<IncludeRuleComponent> builder)
    {
        builder.ToTable("element_component_include_rule");

        builder.Property(x => x.IncludeElement)
            .IsRequired()
            .HasConversion(m => m.Value, v => new ElementId(v))
            .HasColumnName("include_element_id");

        builder.Property(x => x.LevelRequirement)
            .IsRequired()
            .HasColumnName("level_requirement");

        builder.Property(x => x.Requirements)
            .HasColumnName("requirements");
    }
}
