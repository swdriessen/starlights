using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class PrimaryAbilityComponentTypeConfiguration : IEntityTypeConfiguration<PrimaryAbilityComponent>
{
    public void Configure(EntityTypeBuilder<PrimaryAbilityComponent> builder)
    {
        builder.ToTable("element_component_primary_ability");
        builder.Property(x => x.PrimaryAbility)
            .IsRequired();
    }
}
