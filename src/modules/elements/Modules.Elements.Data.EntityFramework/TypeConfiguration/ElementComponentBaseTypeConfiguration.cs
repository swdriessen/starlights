using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class ElementComponentBaseTypeConfiguration : IEntityTypeConfiguration<ElementComponentBase>
{
    public void Configure(EntityTypeBuilder<ElementComponentBase> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(s => s.Id)
           .ValueGeneratedNever()
           .HasConversion(m => m, v => v);

        builder.UseTpcMappingStrategy();
    }
}
