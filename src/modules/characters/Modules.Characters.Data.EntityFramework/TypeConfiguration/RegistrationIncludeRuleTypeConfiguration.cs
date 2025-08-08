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
               .ValueGeneratedNever()
               .HasConversion(m => m.Value, v => new RegistrationIncludeRuleId(v));

        builder.Property(e => e.ParentRegistrationId)
               .IsRequired()
               .HasConversion(m => m.Value, v => new RegistrationId(v));

        builder.Property(e => e.IncludedElementId)
               .IsRequired()
               .HasConversion(m => m.Value, v => new ElementId(v));

        builder.Property(e => e.AssociatedIncludeRuleId)
               .IsRequired()
               .HasConversion(m => m.Value, v => new ElementComponentId(v));

        builder.Property(e => e.IncludedElementName)
               .IsRequired();


        // Foreign key relationship to Registration
        builder.HasIndex(e => e.ParentRegistrationId);

    }
}
