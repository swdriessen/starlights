using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration;

public class RegistrationTypeConfiguration : IEntityTypeConfiguration<Registration>
{
    public void Configure(EntityTypeBuilder<Registration> builder)
    {
        builder.ToTable("registration");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(m => m.Value, v => new RegistrationId(v));

        builder.Property(e => e.ParentRegistrationId)
            .HasColumnName("parent_registration_id")
            .IsRequired(false)
            .HasConversion(m => m.HasValue ? m.Value.Value : (Guid?)null, v => v.HasValue ? new RegistrationId(v.Value) : null);

        builder.Property(e => e.CharacterId)
            .HasColumnName("character_id")
            .IsRequired()
            .HasConversion(m => m.Value, v => new CharacterId(v));

        builder.HasIndex(e => e.CharacterId);

        builder.Property(e => e.AssociatedElementId)
            .HasColumnName("associated_element_id")
            .IsRequired()
            .HasConversion(m => m.Value, v => new ElementId(v));

        builder.Property(e => e.AssociatedElementName)
            .HasColumnName("associated_element_name")
            .HasMaxLength(128);

        builder.Property(e => e.AssociatedElementType)
            .HasColumnName("associated_element_type")
            .HasMaxLength(128);

        builder.Property(e => e.ProgressionOriginRegistrationId)
            .HasColumnName("progression_origin_registration_id")
            .IsRequired(false)
            .HasConversion(m => m.HasValue ? m.Value.Value : (Guid?)null, v => v.HasValue ? new RegistrationId(v.Value) : null);

        builder.HasMany(x => x.IncludeRules)
            .WithOne()
            .HasForeignKey(x => x.ParentRegistrationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasMany(x => x.SelectionRules)
            .WithOne()
            .HasForeignKey(x => x.ParentRegistrationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasMany(x => x.StatisticRules)
            .WithOne()
            .HasForeignKey(x => x.ParentRegistrationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.Property(e => e.OriginatingRule)
            .HasColumnName("originating_rule_id")
            .IsRequired(false);

    }
}
