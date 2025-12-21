using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class SpellAttributesComponentTypeConfiguration : IEntityTypeConfiguration<SpellAttributesComponent>
{
    public void Configure(EntityTypeBuilder<SpellAttributesComponent> builder)
    {
        builder.ToTable("element_component_spell_attributes");

        builder.Property(x => x.Level)
            .HasColumnName("level")
            .IsRequired();

        builder.Property(x => x.MagicSchool)
            .HasColumnName("magic_school")
            .IsRequired();

        builder.Property(x => x.Range)
            .HasColumnName("range")
            .IsRequired();

        builder.Property(x => x.CastingTime)
            .HasColumnName("casting_time")
            .IsRequired();

        builder.Property(x => x.Duration)
            .HasColumnName("duration")
            .IsRequired();

        builder.Property(x => x.IsConcentrationRequired)
            .HasColumnName("is_concentration_required")
            .HasDefaultValue(false);

        builder.Property(x => x.IsRitual)
            .HasColumnName("is_ritual")
            .HasDefaultValue(false);

        builder.Property(x => x.HasSomaticComponent)
            .HasColumnName("has_somatic_component")
            .HasDefaultValue(false);

        builder.Property(x => x.HasVerbalComponent)
            .HasColumnName("has_verbal_component")
            .HasDefaultValue(false);

        builder.Property(x => x.HasMaterialComponent)
            .HasColumnName("has_material_component")
            .HasDefaultValue(false);

        builder.Property(x => x.MaterialComponentsDescription)
            .HasColumnName("material_components_description")
            .IsRequired(false);

    }
}