using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Classes;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration;

public class ClassComponentTypeConfiguration : IEntityTypeConfiguration<ClassComponent>
{
    public void Configure(EntityTypeBuilder<ClassComponent> builder)
    {
        // TPC - each derived component mapped to its own table
        builder.ToTable("character_component_class");

        // Configure one-to-many relationship to CharacterClass with cascade delete
        builder.HasMany(x => x.Classes)
               .WithOne()
               .HasForeignKey("ClassComponentId")
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();
    }
}
