using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Values;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class AbilityComponentTypeConfiguration : IEntityTypeConfiguration<AbilityComponent>
{
    public void Configure(EntityTypeBuilder<AbilityComponent> builder)
    {
        builder.ToTable("element_component_ability");

        builder.Property(x => x.Abbreviation)
            .HasConversion(v => v.Value, v => new Abbreviation(v))
            .IsRequired()
            .HasColumnName("abbreviation");
    }
}
