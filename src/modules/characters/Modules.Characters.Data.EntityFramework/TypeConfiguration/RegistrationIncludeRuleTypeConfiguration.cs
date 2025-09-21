using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration;

public class RegistrationIncludeRuleTypeConfiguration : IEntityTypeConfiguration<RegistrationIncludeRule>
{
    public void Configure(EntityTypeBuilder<RegistrationIncludeRule> builder)
    {
        builder.ToTable("registration_include_rules");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(m => m.Value, v => new RegistrationIncludeRuleId(v));

        builder.Property(e => e.ParentRegistrationId)
            .HasColumnName("parent_registration_id")
            .IsRequired()
            .HasConversion(m => m.Value, v => new RegistrationId(v));

        builder.HasIndex(e => e.ParentRegistrationId);

        builder.Property(e => e.IncludedElementId)
            .HasColumnName("included_element_id")
            .IsRequired()
            .HasConversion(m => m.Value, v => new ElementId(v));

        builder.Property(e => e.AssociatedIncludeRuleId)
            .HasColumnName("associated_include_rule_id")
            .IsRequired()
            .HasConversion(m => m.Value, v => new ElementComponentId(v));

        builder.Property(e => e.IncludedElementName)
            .HasColumnName("included_element_name")
            .IsRequired();
    }
}
