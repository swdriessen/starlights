using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class LanguageComponentTypeConfiguration : IEntityTypeConfiguration<LanguageComponent>
{
    public void Configure(EntityTypeBuilder<LanguageComponent> builder)
    {
        builder.ToTable("element_component_language");

        builder.Property(x => x.Origin)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Kind)
            .IsRequired()
            .HasMaxLength(32);
    }
}
