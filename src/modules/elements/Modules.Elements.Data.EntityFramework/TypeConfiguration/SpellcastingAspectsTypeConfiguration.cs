using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components.Spellcasting;
using Starlights.Platform.Components.Data.EntityFramework.Extensions;
using Range = Starlights.Modules.Elements.Domain.Components.Spellcasting.Range;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class SpellcastingAspectsTypeConfiguration : IEntityTypeConfiguration<SpellcastingAspects>
{
    public void Configure(EntityTypeBuilder<SpellcastingAspects> builder)
    {
        builder.ToTable("element_component_aspect_spellcasting");

        builder.Property(x => x.Classification)
            .HasJsonConversion<SpellClassification>()
            .HasColumnType("nvarchar(max)")
            .HasColumnName("level")
            .IsRequired();

        builder.Property(x => x.Range)
            .HasJsonConversion<Range>()
            .HasColumnType("nvarchar(max)")
            .HasColumnName("range")
            .IsRequired();

        builder.Property(x => x.CastingTime)
            .HasJsonConversion<CastingTime>()
            .HasColumnType("nvarchar(max)")
            .HasColumnName("casting_time")
            .IsRequired();

        builder.Property(x => x.Duration)
            .HasJsonConversion<Duration>()
            .HasColumnType("nvarchar(max)")
            .HasColumnName("duration")
            .IsRequired();

        builder.Property(x => x.Components)
            .HasJsonConversion<SpellComponents>()
            .HasColumnType("nvarchar(max)")
            .HasColumnName("components")
            .IsRequired();

    }
}