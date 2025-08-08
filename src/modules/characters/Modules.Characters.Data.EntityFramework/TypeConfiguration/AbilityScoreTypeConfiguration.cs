using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
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
               .ValueGeneratedNever()
               .HasConversion(m => m.Value, v => new AbilityScoreId(v));

        // Relationship to Character via shadow FK; Character owns many AbilityScores
        builder.Property<CharacterId>("CharacterId").HasConversion(m => m.Value, v => new CharacterId(v));
        builder.HasIndex("CharacterId");

        // Properties
        builder.Property(e => e.AssociatedRegistrationId)
               .IsRequired()
               .HasConversion(m => m.Value, v => new RegistrationId(v));

        builder.HasIndex(e => e.AssociatedRegistrationId);

        builder.Property(e => e.Name)
               .IsRequired();

        builder.Property(e => e.Abbreviation)
               .IsRequired();

        builder.Property(e => e.BaseScore)
               .HasDefaultValue(10);
    }
}
