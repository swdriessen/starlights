using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Appearances;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration.ComponentConfiguration;

public class AppearanceComponentTypeConfiguration : IEntityTypeConfiguration<AppearanceComponent>
{
    public void Configure(EntityTypeBuilder<AppearanceComponent> builder)
    {
        builder.ToTable("character_component_appearance");
    }
}
