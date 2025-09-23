using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class ElementComponentBaseTypeConfiguration : IEntityTypeConfiguration<ElementComponentBase>
{
    public void Configure(EntityTypeBuilder<ElementComponentBase> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
               .ValueGeneratedNever()
               .HasConversion(m => m.Value, v => new ElementComponentId(v))
               .HasColumnName("id");

        builder.Property(e => e.OwningElement)
               .IsRequired()
               .HasConversion(m => m.Value, v => new ElementId(v))
               .HasColumnName("owning_element_id");

        // prefix column name with component_ to avoid potential conflicts or confusion
        builder.Property(e => e.OrderSequence)
               .IsRequired()
               .HasColumnName("component_order_sequence");

        builder.HasIndex(e => new { e.OwningElement, e.OrderSequence })
               .IsUnique();

        builder.UseTpcMappingStrategy();
    }
}
