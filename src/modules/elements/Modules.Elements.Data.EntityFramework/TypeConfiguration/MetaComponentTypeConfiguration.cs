using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Components.Data.EntityFramework.Extensions;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public sealed class MetaComponentTypeConfiguration : IEntityTypeConfiguration<MetaComponent>
{
    public void Configure(EntityTypeBuilder<MetaComponent> builder)
    {
        builder.ToTable("element_component_meta");

        builder.Property(x => x.Parent)
            .HasJsonConversion<ParentElement?>()
            .HasColumnType("nvarchar(max)")
            .HasColumnName("parent_element")
            .IsRequired(false);

    }
}
