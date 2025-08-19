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
               .ValueGeneratedNever()
               .HasConversion(m => m.Value, v => new RegistrationStatisticRuleId(v));

        builder.Property(e => e.ParentRegistrationId)
               .IsRequired()
               .HasConversion(m => m.Value, v => new RegistrationId(v));

        builder.Property(e => e.AssociatedStatisticRuleId)
               .IsRequired()
               .HasConversion(m => m.Value, v => new ElementComponentId(v));

        builder.Property(e => e.Name)
               .IsRequired();

        builder.Property(e => e.Value)
               .IsRequired();

        builder.HasIndex(e => e.ParentRegistrationId);
    }
}
