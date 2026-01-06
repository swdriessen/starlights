using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Elements.Domain.Components.Proficiency;

namespace Starlights.Modules.Elements.Data.EntityFramework.TypeConfiguration;

public class ProficiencyAttributesComponentTypeConfiguration : IEntityTypeConfiguration<ProficiencyAttributesComponent>
{
    public void Configure(EntityTypeBuilder<ProficiencyAttributesComponent> builder)
    {
        builder.ToTable("element_component_proficiency_attributes");

        builder.Property(x => x.ProficiencyType)
            .HasColumnName("proficiency_type")
            .IsRequired();
    }
}
