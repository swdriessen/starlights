using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Skills;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration.ComponentConfiguration;

public class SkillsComponentTypeConfiguration : IEntityTypeConfiguration<SkillsComponent>
{
    public void Configure(EntityTypeBuilder<SkillsComponent> builder)
    {
        builder.ToTable("component_skills");

        builder.HasMany(x => x.Skills)
               .WithOne()
               .HasForeignKey("parent_component_id")
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();
    }
}
