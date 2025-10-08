using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Services.Processing;

public interface IRegistrationProcessor
{
    /// <summary>
    /// Processes a registration by its unique identifier.
    /// </summary>
    Task<ProcessRegistrationResult> ProcessRegistration(RegistrationId registrationId);

    /// <summary>
    /// Processes the removal of a registration by its unique identifier.
    /// </summary>
    Task<ProcessRegistrationResult> ProcessUnregistration(RegistrationId registrationId);
}
