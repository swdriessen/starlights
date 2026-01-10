using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components.Class;
using Starlights.Platform.Components.Data.EntityFramework.Extensions;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class ClassAspectsTypeConfiguration : IEntityTypeConfiguration<ClassAspects>
{
    public void Configure(EntityTypeBuilder<ClassAspects> builder)
    {
        builder.ToTable("element_component_aspect_class");

        builder.Property(x => x.HitDice)
            .HasColumnName("hd")
            .HasJsonConversion<HitPointDie>()
            .HasColumnType("nvarchar(max)")
            .IsRequired();
    }
}
