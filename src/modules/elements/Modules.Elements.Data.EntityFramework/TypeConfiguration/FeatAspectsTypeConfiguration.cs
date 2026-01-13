using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components.Feat;
using Starlights.Platform.Components.Data.EntityFramework.Extensions;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class FeatAspectsTypeConfiguration : IEntityTypeConfiguration<FeatAspects>
{
    public void Configure(EntityTypeBuilder<FeatAspects> builder)
    {
        builder.ToTable("element_component_aspect_feat");

        builder.Property(x => x.Category)
            .HasColumnName("category")
            .HasJsonConversion<FeatCategory>()
            .HasColumnType("nvarchar(max)")
            .IsRequired();
    }
}
