using Microsoft.EntityFrameworkCore;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Components.Data.EntityFramework;

namespace Starlights.Modules.Characters.Data.EntityFramework;

internal class RegistrationRepository : RepositoryBase<Registration>, IRegistrationRepository
{
    public void Add(Registration registration) => Entities.Add(registration);

    public Task DeleteRegistrationsAsync(CharacterId id)
    {
        var toRemove = Entities.Where(r => r.CharacterId == id);
        Entities.RemoveRange(toRemove);
        return Task.CompletedTask;
    }

    public async Task<Registration?> GetRegistrationAsync(RegistrationId id)
    {
        return await Entities
            .Include(x => x.SelectionRules)
            .Include(x => x.IncludeRules)
            .Include(x => x.StatisticRules)
            .SingleOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Registration>> GetRegistrationsAsync(CharacterId id)
    {
        return await Entities
            .Include(x => x.SelectionRules)
            .Include(x => x.IncludeRules)
            .Include(x => x.StatisticRules)
            .Where(r => r.CharacterId == id)
            .ToListAsync();
    }

    public async Task<List<Registration>> GetRegistrationsByAssociationsAsync(CharacterId id, ElementId associatedElementId)
    {
        return await Entities
            .Include(x => x.SelectionRules)
            .Include(x => x.IncludeRules)
            .Include(x => x.StatisticRules)
            .Where(r => r.CharacterId == id && r.AssociatedElementId == associatedElementId)
            .ToListAsync();
    }
}