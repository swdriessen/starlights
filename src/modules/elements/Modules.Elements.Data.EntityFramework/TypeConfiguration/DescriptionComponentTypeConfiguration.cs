using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class DescriptionComponentTypeConfiguration : IEntityTypeConfiguration<DescriptionComponent>
{
    public void Configure(EntityTypeBuilder<DescriptionComponent> builder)
    {
        builder.ToTable("element_component_description");
    }
}
