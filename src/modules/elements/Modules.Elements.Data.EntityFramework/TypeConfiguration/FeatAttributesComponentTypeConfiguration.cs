using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class FeatAttributesComponentTypeConfiguration : IEntityTypeConfiguration<FeatAttributesComponent>
{
    public void Configure(EntityTypeBuilder<FeatAttributesComponent> builder)
    {
        builder.ToTable("element_component_feat_attributes");

        builder.Property(x => x.CategoryId)
            .HasColumnName("category_id")
            .IsRequired();

        builder.Property(x => x.Category)
            .HasColumnName("category")
            .IsRequired();
    }
}
