using Microsoft.EntityFrameworkCore;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Components.Data.EntityFramework;

namespace Starlights.Modules.Characters.Data.EntityFramework;

internal class RegistrationRepository : RepositoryBase<Registration>, IRegistrationRepository
{
    public void Add(Registration registration)
    {
        using var _ = CharactersInstrumentation.StartActivity("Add Registration");
        Entities.Add(registration);
    }
    public async Task<Registration?> GetRegistrationAsync(RegistrationId id)
    {
        using var _ = CharactersInstrumentation.StartActivity();
        return await Entities.SingleOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Registration>> GetRegistrationsAsync(CharacterId id)
    {
        using var _ = CharactersInstrumentation.StartActivity();
        return await Entities.Where(r => r.CharacterId == id).ToListAsync();
    }
}