using Starlights.Modules.Characters.Domain;
using Starlights.Platform.Components.Data.EntityFramework;

namespace Starlights.Modules.Characters.Data.EntityFramework;

internal class CharactersRepository : RepositoryBase<Character>, ICharactersRepository
{
    public async Task AddAsync(Character character)
    {
        using var _ = CharactersInstrumentation.StartActivity();
        await Entities.AddAsync(character);
    }

    public async Task<Character?> GetCharacterAsync(Guid identifier)
    {
        using var _ = CharactersInstrumentation.StartActivity();
        return await Entities.FindAsync(identifier);
    }
}
