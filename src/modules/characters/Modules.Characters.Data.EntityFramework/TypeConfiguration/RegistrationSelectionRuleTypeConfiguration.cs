using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration;

public class RegistrationSelectionRuleTypeConfiguration : IEntityTypeConfiguration<RegistrationSelectionRule>
{
    public void Configure(EntityTypeBuilder<RegistrationSelectionRule> builder)
    {
        builder.ToTable("registration_selection_rules");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(m => m.Value, v => new RegistrationSelectionRuleId(v));

        builder.Property(e => e.ParentRegistrationId)
            .HasColumnName("parent_registration_id")
            .IsRequired()
            .HasConversion(m => m.Value, v => new RegistrationId(v));

        builder.Property(e => e.AssociatedSelectionRuleId)
            .HasColumnName("associated_selection_rule_id")
            .IsRequired()
            .HasConversion(m => m.Value, v => new ElementComponentId(v));

        builder.Property(e => e.SelectedOption)
            .HasColumnName("current_selection")
            .IsRequired(false)
            .HasConversion(m => m.HasValue ? m.Value.Value : (Guid?)null, v => v.HasValue ? new ElementId(v.Value) : null);

        builder.Property(e => e.SelectionRegistrationId)
            .HasColumnName("current_selection_registration_id")
            .IsRequired(false)
            .HasConversion(m => m.HasValue ? m.Value.Value : (Guid?)null, v => v.HasValue ? new RegistrationId(v.Value) : null);

        builder.Property(e => e.ElementType)
            .HasColumnName("element_type")
            .IsRequired();

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.HasIndex(e => e.ParentRegistrationId);
    }
}
