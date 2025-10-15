using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration;

public class RegistrationStatisticRuleTypeConfiguration : IEntityTypeConfiguration<RegistrationStatisticRule>
{
    public void Configure(EntityTypeBuilder<RegistrationStatisticRule> builder)
    {
        builder.ToTable("registration_statistic_rules");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(m => m.Value, v => new RegistrationStatisticRuleId(v));

        builder.Property(e => e.ParentRegistrationId)
            .HasColumnName("parent_registration_id")
            .IsRequired()
            .HasConversion(m => m.Value, v => new RegistrationId(v));

        builder.Property(e => e.AssociatedStatisticRuleId)
            .HasColumnName("associated_statistic_rule_id")
            .IsRequired()
            .HasConversion(m => m.Value, v => new ElementComponentId(v));

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(e => e.Value)
            .HasColumnName("value")
            .IsRequired();

        builder.Property(e => e.LevelRequirement)
            .HasColumnName("level_requirement")
            .IsRequired();

        builder.Property(e => e.StackingBonus)
            .HasColumnName("stacking_bonus")
            .IsRequired();

        builder.Property(e => e.MinimumValue)
            .HasColumnName("min_value")
            .IsRequired(false);

        builder.Property(e => e.MaximumValue)
            .HasColumnName("max_value")
            .IsRequired(false);

        builder.HasIndex(e => e.ParentRegistrationId);
    }
}
