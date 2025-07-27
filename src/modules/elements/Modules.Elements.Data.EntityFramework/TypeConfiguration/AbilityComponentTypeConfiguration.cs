using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class AbilityComponentTypeConfiguration : IEntityTypeConfiguration<AbilityComponent>
{
    public void Configure(EntityTypeBuilder<AbilityComponent> builder)
    {
        builder.ToTable("element_component_ability");
    }
}
