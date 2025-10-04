using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Progression;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration.ComponentConfiguration;

public class ProgressionComponentTypeConfiguration : IEntityTypeConfiguration<ProgressionComponent>
{
    public void Configure(EntityTypeBuilder<ProgressionComponent> builder)
    {
        builder.ToTable("component_progression");

        builder.Property(x => x.CharacterLevel)
            .IsRequired()
            .HasColumnName("character_level");
    }
}

public class ProficiencyComponentTypeConfiguration : IEntityTypeConfiguration<ProficiencyComponent>
{
    public void Configure(EntityTypeBuilder<ProficiencyComponent> builder)
    {
        builder.ToTable("component_proficiency");

        builder.Property(x => x.ProficiencyBonus)
            .IsRequired()
            .HasColumnName("proficiency_bonus");
    }
}