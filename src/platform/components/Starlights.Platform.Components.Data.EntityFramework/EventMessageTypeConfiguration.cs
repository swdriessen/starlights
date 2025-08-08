using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Starlights.Platform.Components.Data.EntityFramework;

/// <summary>
/// Entity Framework Core configuration for the <see cref="EventMessage"/> entity.
/// </summary>
public class EventMessageTypeConfiguration : IEntityTypeConfiguration<EventMessage>
{
    public void Configure(EntityTypeBuilder<EventMessage> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
               .ValueGeneratedNever();

        builder.Property(e => e.EventType)
            .IsRequired();

        builder.Property(e => e.Payload)
            .IsRequired();

        builder.Property(e => e.OccurredOn)
            .IsRequired();
    }
}
