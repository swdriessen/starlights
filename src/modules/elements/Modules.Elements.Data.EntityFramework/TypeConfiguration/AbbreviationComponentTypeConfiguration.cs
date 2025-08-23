using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class AbbreviationComponentTypeConfiguration : IEntityTypeConfiguration<AbbreviationComponent>
{
    public void Configure(EntityTypeBuilder<AbbreviationComponent> builder)
    {
        builder.ToTable("element_component_abbreviation");
        builder.Property(x => x.Abbreviation)
            .IsRequired()
            .HasMaxLength(32)
            .HasColumnName("abbreviation");
    }
}
