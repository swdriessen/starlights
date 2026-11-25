using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Starlights.Modules.FileManagement.Data;

internal sealed class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.ToTable("files");
        builder.HasKey(file => file.Id);
        builder.Property(file => file.Id).ValueGeneratedNever();
        builder.Property(file => file.FileName).HasMaxLength(255);
        builder.Property(file => file.ContentType).HasMaxLength(100);
        builder.Property(file => file.CreatedAt).HasConversion(value => value.UtcTicks, value => new DateTimeOffset(value, TimeSpan.Zero));
        builder.HasIndex(file => new { file.OwnerId, file.CreatedAt, file.Id });
    }
}
