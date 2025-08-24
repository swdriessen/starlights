using Starlights.Modules.Characters.Domain.Appearances;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Data;

public interface IAppearanceRepository : IRepository
{
    /// <summary>
    /// Adds a new appearance to the repository.
    /// </summary>
    void Add(Appearance appearance);

    /// <summary>
    /// Retrieves an appearance by its identifier.
    /// </summary>
    Task<Appearance?> GetAppearanceAsync(AppearanceId id);

    /// <summary>
    /// Retrieves an appearance for a specific character.
    /// </summary>
    Task<Appearance?> GetAppearanceAsync(CharacterId id);


    Task DeleteAppearanceAsync(CharacterId id);
}
