using Microsoft.EntityFrameworkCore;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Platform.Components.Data.EntityFramework;

namespace Starlights.Modules.Characters.Data.EntityFramework;

internal class CharactersRepository : RepositoryBase<Character>, ICharactersRepository
{
    public void Add(Character character)
    {
        using var _ = CharactersInstrumentation.StartActivity("Add Character");
        Entities.Add(character);
    }

    public async Task<Character?> GetCharacterAsync(Guid identifier)
    {
        using var _ = CharactersInstrumentation.StartActivity();
        return await Entities
            .Include(c => c.AbilityScores)
            .Include(c => c.Skills)
            .Include(c => c.SavingThrows)
            .SingleOrDefaultAsync(a => a.Id == identifier);
    }
}
