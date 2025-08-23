using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Services.Processing;

public interface IRegistrationManager
{
    /// <summary>
    /// Processes a registration by its unique identifier.
    /// </summary>
    Task<ProcessRegistrationResult> ProcessRegistration(RegistrationId registrationId);
}
