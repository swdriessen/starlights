using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration;

public class AppearanceTypeConfiguration : IEntityTypeConfiguration<Appearance>
{
    public void Configure(EntityTypeBuilder<Appearance> builder)
    {
        builder.ToTable("appearance");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
               .ValueGeneratedNever()
               .HasConversion(m => m.Value, v => new AppearanceId(v));

        builder.Property(e => e.CharacterId)
               .IsRequired()
               .HasConversion(m => m.Value, v => new CharacterId(v));

        builder.Property(e => e.PortraitUrl)
               .HasMaxLength(2048);

        // Foreign key relationship to Character
        builder.HasIndex(e => e.CharacterId);
    }
}
