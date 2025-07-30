using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class ElementTypeConfiguration : IEntityTypeConfiguration<Element>
{
    public void Configure(EntityTypeBuilder<Element> builder)
    {
        builder.ToTable("element");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
               .ValueGeneratedNever()
               .HasConversion(m => m.Value, v => new ElementId(v));

        builder.Property(e => e.Name)
            .IsRequired();

        builder.Property(e => e.Type)
            .IsRequired();

        builder.HasMany(x => x.Components)
            .WithOne()
            .HasForeignKey(x => x.OwningElement)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}
