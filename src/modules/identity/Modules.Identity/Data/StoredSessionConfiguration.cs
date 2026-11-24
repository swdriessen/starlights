using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Starlights.Modules.Identity.Data;

internal sealed class StoredSessionConfiguration : IEntityTypeConfiguration<StoredSession>
{
    public void Configure(EntityTypeBuilder<StoredSession> builder)
    {
        builder.ToTable("Sessions");
        builder.HasKey(session => session.Id);
        builder.Property(session => session.Id).HasMaxLength(32);
        builder.Property(session => session.ProtectedTicket).IsRequired();
        builder.HasIndex(session => session.ExpiresAt);
    }
}
