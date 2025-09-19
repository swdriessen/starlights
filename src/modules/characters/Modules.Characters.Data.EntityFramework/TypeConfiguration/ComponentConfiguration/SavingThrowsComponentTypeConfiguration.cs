using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.SavingThrows;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration.ComponentConfiguration;

public class SavingThrowsComponentTypeConfiguration : IEntityTypeConfiguration<SavingThrowsComponent>
{
    public void Configure(EntityTypeBuilder<SavingThrowsComponent> builder)
    {
        builder.ToTable("character_component_saving_throws");

        builder.HasMany(x => x.SavingThrows)
               .WithOne()
               .HasForeignKey("component_id")
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();
    }
}
