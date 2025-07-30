using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public sealed class PrerequisitesComponentTypeConfiguration : IEntityTypeConfiguration<PrerequisitesComponent>
{
    public void Configure(EntityTypeBuilder<PrerequisitesComponent> builder)
    {
        builder.Property(x => x.Prerequisites)
            .IsRequired()
            .HasMaxLength(256);
    }
}
