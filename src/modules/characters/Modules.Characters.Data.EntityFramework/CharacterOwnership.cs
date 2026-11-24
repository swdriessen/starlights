using Microsoft.EntityFrameworkCore;
using Starlights.Modules.Characters.Domain.Characters;

namespace Starlights.Modules.Characters.Data.EntityFramework;

internal sealed class CharacterOwnership(IDbContextFactory<CharactersContext> factory) : ICharacterOwnership
{
    public async Task<bool> IsOwnerAsync(Guid characterId, Guid userId, CancellationToken cancellationToken)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken);
        return await context.Set<Character>().AnyAsync(character =>
            character.Id == new CharacterId(characterId) && EF.Property<Guid>(character, "OwnerId") == userId,
            cancellationToken);
    }
}
