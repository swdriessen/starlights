using Microsoft.EntityFrameworkCore;
using Starlights.Modules.Characters.Domain;
using Starlights.Platform.Components.Data.EntityFramework;

namespace Starlights.Modules.Characters.Data.EntityFramework;

internal class AppearanceRepository : RepositoryBase<Appearance>, IAppearanceRepository
{
    public void Add(Appearance appearance)
    {
        using var _ = CharactersInstrumentation.StartActivity();
        Entities.Add(appearance);
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