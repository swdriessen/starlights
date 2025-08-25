using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Progression;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration;

public class ProgressionComponentTypeConfiguration : IEntityTypeConfiguration<ProgressionComponent>
{
    public void Configure(EntityTypeBuilder<ProgressionComponent> builder)
    {
        builder.ToTable("character_progression");

        builder.Property(x => x.CharacterLevel)
            .IsRequired()
            .HasColumnName("character_level");
    }
}
