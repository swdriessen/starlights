using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Abilities;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration.ComponentConfiguration;

public class AbilitiesComponentTypeConfiguration : IEntityTypeConfiguration<AbilitiesComponent>
{
    public void Configure(EntityTypeBuilder<AbilitiesComponent> builder)
    {
        builder.ToTable("character_component_abilities");

        builder.HasMany(x => x.AbilityScores)
               .WithOne()
               .HasForeignKey("component_id")
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();
    }
}
