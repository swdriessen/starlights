using Microsoft.EntityFrameworkCore;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Platform.Components.Data.EntityFramework;

namespace Starlights.Modules.Characters.Data.EntityFramework;

internal class CharactersRepository : RepositoryBase<Character>, ICharactersRepository
{
    public void Add(Character character) => Entities.Add(character);

    public Task DeleteCharacterAsync(CharacterId identifier)
    {
        var toRemove = Entities.SingleOrDefault(c => c.Id == identifier);
        if (toRemove != null)
        {
            Entities.Remove(toRemove);
        }
        return Task.CompletedTask;
    }

    public async Task<Character?> GetCharacterAsync(Guid identifier)
    {
        return await Entities
            .Include(c => c.AbilityScores)
            .Include(c => c.Skills)
            .Include(c => c.SavingThrows)
            .Include(c => c.Components)
            .SingleOrDefaultAsync(a => a.Id == identifier);
    }

    public async Task<IEnumerable<Character>> GetCharactersAsync()
    {
        return await Entities
            .Include(c => c.AbilityScores)
            .Include(c => c.Skills)
            .Include(c => c.SavingThrows)
            .Include(c => c.Components)
            .ToListAsync();
    }
}
