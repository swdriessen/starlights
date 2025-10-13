using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Services.Processing;

public interface IRegistrationProcessor
{
    /// <summary>
    /// Processes a registration by its unique identifier.
    /// </summary>
    Task<ProcessRegistrationResult> ProcessRegistration(RegistrationId registrationId);

    /// <summary>
    /// Reprocesses all registrations associated with the specified character asynchronously.
    /// </summary>
    Task<ProcessRegistrationResult> ReproccessRegistrations(CharacterId characterId);
}

