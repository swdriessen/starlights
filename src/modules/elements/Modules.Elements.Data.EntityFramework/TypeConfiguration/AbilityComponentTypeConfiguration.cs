using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components.Ability;
using Starlights.Modules.Elements.Domain.Values;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class AbilityComponentTypeConfiguration : IEntityTypeConfiguration<AbilityAspects>
{
    public void Configure(EntityTypeBuilder<AbilityAspects> builder)
    {
        builder.ToTable("element_component_aspect_ability");

        builder.Property(x => x.Abbreviation)
            .HasConversion(v => v.Value, v => new Abbreviation(v))
            .IsRequired()
            .HasColumnName("abbreviation");
    }
}
