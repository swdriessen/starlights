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
               .ValueGeneratedNever()
               .HasConversion(m => m.Value, v => new RegistrationId(v));

        builder.Property(e => e.ParentRegistrationId)
               .IsRequired(false)
               .HasConversion(
                m => m.HasValue ? m.Value.Value : (Guid?)null,
                v => v.HasValue ? new RegistrationId(v.Value) : null);

        builder.Property(e => e.CharacterId)
               .IsRequired()
               .HasConversion(m => m.Value, v => new CharacterId(v));

        builder.Property(e => e.AssociatedElementId)
               .IsRequired()
               .HasConversion(m => m.Value, v => new ElementId(v));

        builder.Property(e => e.AssociatedElementName)
               .HasMaxLength(128);

        builder.HasMany(x => x.IncludeRules)
            .WithOne()
            .HasForeignKey(x => x.ParentRegistrationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // Foreign key relationship to Character
        builder.HasIndex(e => e.CharacterId);
    }
}
