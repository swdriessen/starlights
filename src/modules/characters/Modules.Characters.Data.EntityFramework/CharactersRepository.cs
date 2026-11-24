using Microsoft.EntityFrameworkCore;
using Starlights.Modules.Characters.Data.EntityFramework.Querying;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Platform.Components.Data.EntityFramework;
using Starlights.Platform.Hosting;

namespace Starlights.Modules.Characters.Data.EntityFramework;

internal class CharactersRepository(ICurrentUser currentUser) : RepositoryBase<Character>, ICharactersRepository
{
    public void Add(Character character) => Entities.Add(character);

    public Task<bool> DeleteCharacterAsync(CharacterId identifier)
    {
        var toRemove = Entities.SingleOrDefault(c => c.Id == identifier);
        if (toRemove is null)
        {
            return Task.FromResult(false);
        }

        Entities.Remove(toRemove);
        return Task.FromResult(true);
    }

    public async Task<Character?> GetCharacterAsync(Guid identifier)
    {
        return await Entities
            .IncludeAggregateGraph()
            .AsSplitQuery()
            .SingleOrDefaultAsync(a => a.Id == identifier);
    }

    public async Task<IEnumerable<Character>> GetCharactersAsync()
    {
        var userId = currentUser.UserId
            ?? throw new InvalidOperationException("Listing Characters requires an authenticated owner.");
        return await Entities
            .Where(character => EF.Property<Guid>(character, "OwnerId") == userId)
            .IncludeAggregateGraph()
            .AsSplitQuery()
            .ToListAsync();
    }
}
