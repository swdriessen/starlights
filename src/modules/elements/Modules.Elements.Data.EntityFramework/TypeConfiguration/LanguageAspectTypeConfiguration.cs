using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components.Language;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class LanguageAspectTypeConfiguration : IEntityTypeConfiguration<LanguageAspects>
{
    public void Configure(EntityTypeBuilder<LanguageAspects> builder)
    {
        builder.ToTable("element_component_aspect_language");

        builder.Property(x => x.Origin)
            .IsRequired()
            .HasMaxLength(128)
            .HasColumnName("origin");

        builder.Property(x => x.Classification)
            .HasConversion(x => x.Kind, value => new LanguageClassification(value))
            .IsRequired()
            .HasMaxLength(32)
            .HasColumnName("kind");
    }
}
