using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Classes;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration.ComponentConfiguration;

public class ClassComponentTypeConfiguration : IEntityTypeConfiguration<ClassComponent>
{
    public void Configure(EntityTypeBuilder<ClassComponent> builder)
    {
        builder.ToTable("character_component_class");

        builder.HasMany(x => x.Classes)
               .WithOne()
               .HasForeignKey("component_id")
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();
    }
}
