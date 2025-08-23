using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Characters;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration;

public class CharacterTypeConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.ToTable("character");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
               .ValueGeneratedNever()
                .HasConversion(m => m.Value, v => new CharacterId(v));

        builder.Property(e => e.Name)
            .IsRequired();

        // Configure one-to-many relationship to AbilityScores with cascade delete
        builder.HasMany(x => x.AbilityScores)
               .WithOne()
               .HasForeignKey("CharacterId")
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();





        builder.HasMany(x => x.Components)
            .WithOne()
            .HasForeignKey(x => x.ParentCharacter)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}
