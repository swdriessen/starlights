using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.Components;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration;

public class CharacterClassTypeConfiguration : IEntityTypeConfiguration<CharacterClass>
{
    public void Configure(EntityTypeBuilder<CharacterClass> builder)
    {
        builder.ToTable("character_class");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
               .ValueGeneratedNever()
               .HasConversion(m => m.Value, v => new CharacterClassId(v));

        // Parent component FK (shadow)
        builder.Property<CharacterComponentBaseId>("ClassComponentId")
               .HasConversion(m => m.Value, v => new CharacterComponentBaseId(v));
        builder.HasIndex("ClassComponentId");

        builder.Property(e => e.Registration)
               .IsRequired()
               .HasConversion(m => m.Value, v => new RegistrationId(v));
        builder.HasIndex(e => e.Registration);

        builder.Property(e => e.Name)
               .IsRequired();

        builder.Property(e => e.IsPrimary)
               .IsRequired();

        builder.Property(e => e.Level)
               .HasDefaultValue(1)
               .IsRequired();
    }
}
