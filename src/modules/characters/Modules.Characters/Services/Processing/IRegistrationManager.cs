using Starlights.Modules.Characters.Domain.Registrations;

namespace Modules.Characters.Services.Processing;

public interface IRegistrationManager
{
    Task<int> ProcessRegistration(RegistrationId registrationId);
}
