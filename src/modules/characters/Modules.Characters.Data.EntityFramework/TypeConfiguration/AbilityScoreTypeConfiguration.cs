using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration;

/// <summary>
/// EF Core configuration for <see cref="AbilityScore"/>.
/// </summary>
public class AbilityScoreTypeConfiguration : IEntityTypeConfiguration<AbilityScore>
{
    public void Configure(EntityTypeBuilder<AbilityScore> builder)
    {
        builder.ToTable("ability_scores");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(m => m.Value, v => new AbilityScoreId(v));

        builder.Property(e => e.AssociatedRegistrationId)
            .HasColumnName("associated_registration_id")
            .IsRequired()
            .HasConversion(m => m.Value, v => new RegistrationId(v));

        builder.HasIndex(e => e.AssociatedRegistrationId);

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(e => e.Abbreviation)
            .HasColumnName("abbreviation")
            .IsRequired();

        builder.Property(e => e.BaseScore)
            .HasColumnName("base_score")
            .HasDefaultValue(10);

        builder.Property(e => e.AdditionalScore)
            .HasColumnName("additional_score")
            .HasDefaultValue(0);

        builder.Property(e => e.CalculatedScore)
            .HasColumnName("calculated_score")
            .HasDefaultValue(10);

        builder.Property(e => e.CalculatedModifier)
            .HasColumnName("calculated_modifier")
            .HasDefaultValue(0);
    }
}
