using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration;

public class CharacterClassTypeConfiguration : IEntityTypeConfiguration<CharacterClass>
{
    public void Configure(EntityTypeBuilder<CharacterClass> builder)
    {
        builder.ToTable("character_class");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(m => m.Value, v => new CharacterClassId(v));

        builder.Property(e => e.Registration)
            .HasColumnName("registration_id")
            .IsRequired()
            .HasConversion(m => m.Value, v => new RegistrationId(v));

        builder.HasIndex(e => e.Registration);

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(e => e.IsPrimary)
            .HasColumnName("is_primary")
            .IsRequired();

        builder.Property(e => e.Level)
            .HasColumnName("level")
            .HasDefaultValue(1)
            .IsRequired();
    }
}
