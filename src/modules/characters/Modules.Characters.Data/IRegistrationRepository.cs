using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
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
    /// Asynchronously retrieves the registration associated with the specified originating rule identifier.
    /// </summary>    
    Task<Registration?> GetRegistrationByOriginatingRuleAsync(Guid originatingRuleId);

    /// <summary>
    /// Retrieves all registrations for a specific character.
    /// </summary>
    Task<List<Registration>> GetRegistrationsAsync(CharacterId id);

    /// <summary>
    /// Retrieves all registrations for a specific character by association element id.
    /// </summary>
    Task<List<Registration>> GetRegistrationsByAssociationsAsync(CharacterId id, ElementId associatedElementId);

    /// <summary>
    /// Deletes all registrations for a specific character.
    /// </summary>
    Task<bool> DeleteRegistrationsAsync(CharacterId id);

    /// <summary>
    /// Deletes the registration by its identifier.
    /// </summary>
    Task<bool> DeleteRegistrationAsync(RegistrationId id);
}
