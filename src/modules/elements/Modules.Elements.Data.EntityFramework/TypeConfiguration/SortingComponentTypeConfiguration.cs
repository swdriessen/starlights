using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class SortingComponentTypeConfiguration : IEntityTypeConfiguration<SortingComponent>
{
    public void Configure(EntityTypeBuilder<SortingComponent> builder)
    {
        builder.ToTable("element_component_sorting");
    }
}
