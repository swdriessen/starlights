using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Components;

namespace Starlights.Modules.Characters.Data.EntityFramework.TypeConfiguration.ComponentConfiguration;

public class CharacterComponentBaseTypeConfiguration : IEntityTypeConfiguration<CharacterComponentBase>
{
    public void Configure(EntityTypeBuilder<CharacterComponentBase> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
               .ValueGeneratedNever()
               .HasConversion(m => m.Value, v => new CharacterComponentBaseId(v))
               .HasColumnName("id");

        builder.Property(e => e.ParentCharacter)
               .IsRequired()
               .HasConversion(m => m.Value, v => new CharacterId(v))
               .HasColumnName("parent_character");

        builder.UseTpcMappingStrategy();
    }
}
