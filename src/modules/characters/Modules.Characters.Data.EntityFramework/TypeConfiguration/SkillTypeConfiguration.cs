using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.Skills;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration;

/// <summary>
/// EF Core configuration for <see cref="Skill"/>.
/// </summary>
public class SkillTypeConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("skills");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(m => m.Value, v => new SkillId(v));

        builder.Property(e => e.AssociatedRegistrationId)
            .HasColumnName("associated_registration_id")
            .IsRequired()
            .HasConversion(m => m.Value, v => new RegistrationId(v));

        builder.HasIndex(e => e.AssociatedRegistrationId);

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(e => e.AbilityScoreId)
            .HasColumnName("ability_score_id")
            .IsRequired()
            .HasConversion(m => m.Value, v => new AbilityScoreId(v));

        builder.Property(e => e.AbilityScoreAbbreviation)
            .HasColumnName("ability_score_abbreviation")
            .IsRequired(false);

        builder.Property(e => e.AbilityScoreModifier)
            .HasColumnName("ability_score_modifier")
            .HasDefaultValue(0);

        builder.Property(e => e.AdditionalBonus)
            .HasColumnName("additional_bonus")
            .HasDefaultValue(0);

        builder.Property(e => e.CalculatedBonus)
            .HasColumnName("calculated_bonus")
            .HasDefaultValue(0);

        builder.Property(e => e.SortingOrder)
            .HasColumnName("sorting_order")
            .HasDefaultValue(0);
    }
}
