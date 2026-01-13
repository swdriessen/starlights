using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components.Proficiency;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class ProficiencyAspectsTypeConfiguration : IEntityTypeConfiguration<ProficiencyAspects>
{
    public void Configure(EntityTypeBuilder<ProficiencyAspects> builder)
    {
        builder.ToTable("element_component_aspect_proficiency");

        builder.Property(x => x.Classification)
            .HasColumnName("proficiency_type")
            .HasConversion(x => x.Type, x => new ProficiencyClassification(x))
            .IsRequired();
    }
}
