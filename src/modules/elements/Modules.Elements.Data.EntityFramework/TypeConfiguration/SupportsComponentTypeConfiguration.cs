using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class SupportsComponentTypeConfiguration : IEntityTypeConfiguration<SupportsComponent>
{
    public void Configure(EntityTypeBuilder<SupportsComponent> builder)
    {
        builder.ToTable("element_component_supports");

        builder.Property<List<string>>("_supports")
            .HasColumnName("supports")
            .HasColumnType("nvarchar(max)")
            .HasConversion(v => JsonSerializer.Serialize(v), v => JsonSerializer.Deserialize<List<string>>(v) ?? new List<string>());
    }
}
