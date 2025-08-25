using Microsoft.EntityFrameworkCore;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Appearances;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Platform.Components.Data.EntityFramework;

namespace Starlights.Modules.Characters.Data.EntityFramework;

internal class AppearanceRepository : RepositoryBase<Appearance>, IAppearanceRepository
{
    public void Add(Appearance appearance)
    {
        using var _ = CharactersInstrumentation.StartActivity("Add Appearance");
        Entities.Add(appearance);
    }

    public Task<bool> DeleteAppearanceAsync(CharacterId id)
    {
        var toRemove = Entities.SingleOrDefault(x => x.CharacterId == id);
        if (toRemove is null)
        {
            return Task.FromResult(false);
        }

        Entities.RemoveRange(toRemove);
        return Task.FromResult(true);
    }

    public async Task<Appearance?> GetAppearanceAsync(AppearanceId id)
    {
        using var _ = CharactersInstrumentation.StartActivity();
        return await Entities.SingleOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Appearance?> GetAppearanceAsync(CharacterId id)
    {
        using var _ = CharactersInstrumentation.StartActivity();
        return await Entities.SingleOrDefaultAsync(a => a.CharacterId == id);
    }
}
