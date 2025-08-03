using Microsoft.EntityFrameworkCore;
using Starlights.Modules.Characters.Domain;
using Starlights.Platform.Components.Data.EntityFramework;

namespace Starlights.Modules.Characters.Data.EntityFramework;

internal class CharactersRepository : RepositoryBase<Character>, ICharactersRepository
{
    public void Add(Character character)
    {
        using var _ = CharactersInstrumentation.StartActivity();
        Entities.Add(character);
    }

    public async Task<Character?> GetCharacterAsync(Guid identifier)
    {
        using var _ = CharactersInstrumentation.StartActivity();
        return await Entities.SingleOrDefaultAsync(a => a.Id == identifier);
    }
}
