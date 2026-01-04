using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components.Aspects;
using Starlights.Platform.Components.Data.EntityFramework;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class SpellcastingAspectsTypeConfiguration : IEntityTypeConfiguration<SpellcastingAspects>
{
    public void Configure(EntityTypeBuilder<SpellcastingAspects> builder)
    {
        builder.ToTable("element_component_aspects_spellcasting");

        builder.Property(x => x.Classification)
            .HasConversion(JsonValueObjectConverter<SpellClassification>.Default)
            .HasColumnType("nvarchar(max)")
            .HasColumnName("level")
            .IsRequired();

        builder.Property(x => x.Range)
            .HasConversion(JsonValueObjectConverter<SpellcastingRange>.Default)
            .HasColumnType("nvarchar(max)")
            .HasColumnName("range")
            .IsRequired();

        builder.Property(x => x.CastingTime)
            .HasConversion(JsonValueObjectConverter<CastingTime>.Default)
            .HasColumnType("nvarchar(max)")
            .HasColumnName("casting_time")
            .IsRequired();

        builder.Property(x => x.Duration)
            .HasConversion(JsonValueObjectConverter<Duration>.Default)
            .HasColumnType("nvarchar(max)")
            .HasColumnName("duration")
            .IsRequired();

        builder.Property(x => x.Components)
            .HasConversion(JsonValueObjectConverter<SpellComponents>.Default)
            .HasColumnType("nvarchar(max)")
            .HasColumnName("components")
            .IsRequired();

    }
}