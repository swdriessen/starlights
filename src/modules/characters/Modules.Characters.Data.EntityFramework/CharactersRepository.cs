using Starlights.Modules.Characters.Domain;
using Starlights.Platform.Components.Data.EntityFramework;

namespace Starlights.Modules.Characters.Data.EntityFramework;

internal class CharactersRepository : RepositoryBase<Character>, ICharactersRepository
{
    public async Task AddAsync(Character character)
    {
        await Entities.AddAsync(character);
    }

    public async Task<Character?> GetCharacterAsync(Guid identifier)
    {
        return await Entities.FindAsync(identifier);
    }
}
