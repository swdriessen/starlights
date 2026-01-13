using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public sealed class ClassificationsComponentTypeConfiguration : IEntityTypeConfiguration<ClassificationsComponent>
{
    public void Configure(EntityTypeBuilder<ClassificationsComponent> builder)
    {
        builder.ToTable("element_component_classifications");

        var listComparer = new ValueComparer<List<string>>(
            (left, right) => left!.SequenceEqual(right!),
            value => value.Aggregate(0, (hash, item) => hash ^ StringComparer.Ordinal.GetHashCode(item)), value => value.ToList());

        builder.Property<List<string>>("_labels")
            .HasColumnName("labels")
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => JsonSerializer.Serialize(v),
                v => JsonSerializer.Deserialize<List<string>>(v) ?? new List<string>())
            .Metadata.SetValueComparer(listComparer);

        builder.Property<List<string>>("_tags")
            .HasColumnName("tags")
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => JsonSerializer.Serialize(v),
                v => JsonSerializer.Deserialize<List<string>>(v) ?? new List<string>())
            .Metadata.SetValueComparer(listComparer);
    }
}
