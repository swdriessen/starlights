using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
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
               .ValueGeneratedNever()
               .HasConversion(m => m.Value, v => new SkillId(v));

        // Relationship to Character via shadow FK; Character could own many Skills (keep parity with AbilityScore)
        builder.Property<CharacterId>("CharacterId").HasConversion(m => m.Value, v => new CharacterId(v));
        builder.HasIndex("CharacterId");

        // Properties
        builder.Property(e => e.AssociatedRegistrationId)
               .IsRequired()
               .HasConversion(m => m.Value, v => new RegistrationId(v));

        builder.HasIndex(e => e.AssociatedRegistrationId);

        builder.Property(e => e.Name)
               .IsRequired();

        builder.Property(e => e.AbilityScoreId)
               .IsRequired()
               .HasConversion(m => m.Value, v => new AbilityScoreId(v));

        builder.Property(e => e.AbilityScoreAbbreviation)
               .IsRequired(false);

        builder.Property(e => e.AbilityScoreModifier)
               .HasDefaultValue(0);

        builder.Property(e => e.AdditionalBonus)
               .HasDefaultValue(0);

        builder.Property(e => e.CalculatedBonus)
               .HasDefaultValue(0);
    }
}
