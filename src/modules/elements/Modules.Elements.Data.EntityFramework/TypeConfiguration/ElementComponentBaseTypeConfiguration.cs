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
               .HasConversion(m => m.Value, v => new ElementComponentId(v));

        builder.Property(e => e.OwningElement)
               .IsRequired()
               .HasConversion(m => m.Value, v => new ElementId(v));

        builder.UseTpcMappingStrategy();
    }
}
