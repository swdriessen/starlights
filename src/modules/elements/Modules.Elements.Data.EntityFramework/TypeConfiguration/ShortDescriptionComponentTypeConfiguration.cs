using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public sealed class ShortDescriptionComponentTypeConfiguration : IEntityTypeConfiguration<ShortDescriptionComponent>
{
    public void Configure(EntityTypeBuilder<ShortDescriptionComponent> builder)
    {
        builder.ToTable("element_component_short_description");

        builder.Property(x => x.Content)
            .IsRequired();
    }
}
