using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class RepeatableComponentTypeConfiguration : IEntityTypeConfiguration<RepeatableComponent>
{
    public void Configure(EntityTypeBuilder<RepeatableComponent> builder)
    {
        builder.ToTable("element_component_repeatable");

        builder.Property(x => x.IsRepeatable)
            .HasColumnName("is_repeatable")
            .IsRequired();
    }
}
