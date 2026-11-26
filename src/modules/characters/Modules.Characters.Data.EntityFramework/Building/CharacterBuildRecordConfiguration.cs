using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Characters;

namespace Starlights.Modules.Characters.Data.EntityFramework.Building;

internal sealed class CharacterBuildRecordConfiguration : IEntityTypeConfiguration<CharacterBuildRecord>
{
    public void Configure(EntityTypeBuilder<CharacterBuildRecord> builder)
    {
        builder.ToTable("character_build");
        builder.HasKey(record => record.CharacterId);
        builder.Property(record => record.CharacterId).HasColumnName("character_id")
            .HasConversion(id => id.Value, value => new CharacterId(value)).ValueGeneratedNever();
        builder.HasOne(record => record.Character).WithOne()
            .HasForeignKey<CharacterBuildRecord>(record => record.CharacterId).OnDelete(DeleteBehavior.Cascade);
        builder.Property(record => record.PackId).HasColumnName("pack_id").HasMaxLength(200);
        builder.Property(record => record.PackVersion).HasColumnName("pack_version").HasMaxLength(100);
        builder.Property(record => record.CharacterElementId).HasColumnName("character_element_id").HasMaxLength(300);
        builder.Property(record => record.Composition).HasColumnName("composition").IsRequired();
        builder.Property(record => record.Revision).HasColumnName("revision").IsConcurrencyToken().HasDefaultValue(0L);
        builder.Property(record => record.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("ready");
        builder.Property(record => record.OperationId).HasColumnName("operation_id");
        builder.Property(record => record.Failure).HasColumnName("failure").HasMaxLength(200);
        builder.Property(record => record.Sheet).HasColumnName("sheet");
        builder.Property(record => record.SheetRevision).HasColumnName("sheet_revision");
    }
}
