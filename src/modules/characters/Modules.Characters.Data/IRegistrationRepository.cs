using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Data;

public interface IRegistrationRepository : IRepository
{
    /// <summary>
    /// Adds a new registration to the repository.
    /// </summary>
    void Add(Registration registration);

    /// <summary>
    /// Retrieves a registration by its identifier.
    /// </summary>
    Task<Registration?> GetRegistrationAsync(RegistrationId id);

    /// <summary>
    /// Retrieves all registrations for a specific character.
    /// </summary>
    Task<List<Registration>> GetRegistrationsAsync(CharacterId id);
}