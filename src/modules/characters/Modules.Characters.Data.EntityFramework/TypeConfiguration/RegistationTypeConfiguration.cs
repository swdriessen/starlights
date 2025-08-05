using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration;

public class RegistrationTypeConfiguration : IEntityTypeConfiguration<Registration>
{
    public void Configure(EntityTypeBuilder<Registration> builder)
    {
        builder.ToTable("registration");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
               .ValueGeneratedNever()
               .HasConversion(m => m.Value, v => new RegistrationId(v));

        builder.Property(e => e.CharacterId)
               .IsRequired()
               .HasConversion(m => m.Value, v => new CharacterId(v));

        builder.Property(e => e.ElementId)
               .IsRequired()
               .HasConversion(m => m.Value, v => new ElementId(v));

        builder.Property(e => e.ElementName)
               .HasMaxLength(128);

        // Foreign key relationship to Character
        builder.HasIndex(e => e.CharacterId);
    }
}
