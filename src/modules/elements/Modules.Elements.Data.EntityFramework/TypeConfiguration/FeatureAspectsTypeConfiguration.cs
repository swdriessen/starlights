using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components.Class;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public sealed class FeatureAspectsTypeConfiguration : IEntityTypeConfiguration<FeatureAspects>
{
    public void Configure(EntityTypeBuilder<FeatureAspects> builder)
    {
        builder.ToTable("element_component_aspect_feature");

        builder.Property(x => x.Level)
            .HasColumnName("level")
            .IsRequired();

        builder.Property(x => x.ListingOrder)
            .HasColumnName("listing_order")
            .IsRequired();
    }
}
