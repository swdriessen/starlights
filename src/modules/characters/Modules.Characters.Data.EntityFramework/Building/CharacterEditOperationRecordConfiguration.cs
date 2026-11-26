using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Characters;

namespace Starlights.Modules.Characters.Data.EntityFramework.Building;

internal sealed class CharacterEditOperationRecordConfiguration : IEntityTypeConfiguration<CharacterEditOperationRecord>
{
    public void Configure(EntityTypeBuilder<CharacterEditOperationRecord> builder)
    {
        builder.ToTable("character_edit_operations");
        builder.HasKey(record => new { record.CharacterId, record.OperationId });
        builder.Property(record => record.CharacterId).HasColumnName("character_id")
            .HasConversion(id => id.Value, value => new CharacterId(value));
        builder.HasOne<CharacterBuildRecord>().WithMany().HasForeignKey(record => record.CharacterId).OnDelete(DeleteBehavior.Cascade);
        builder.Property(record => record.OperationId).HasColumnName("operation_id").ValueGeneratedNever();
        builder.Property(record => record.Revision).HasColumnName("revision");
        builder.Property(record => record.Fingerprint).HasColumnName("fingerprint").HasMaxLength(64);
        builder.Property(record => record.Status).HasColumnName("status").HasMaxLength(20);
        builder.Property(record => record.Failure).HasColumnName("failure").HasMaxLength(200);
        builder.Property(record => record.Sheet).HasColumnName("sheet");
        builder.HasIndex(record => record.Status);
        builder.HasIndex(record => new { record.CharacterId, record.Revision }).IsUnique();
    }
}
