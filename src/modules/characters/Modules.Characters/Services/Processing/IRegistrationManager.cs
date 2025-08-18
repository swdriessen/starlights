using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Services.Processing;

public interface IRegistrationManager
{
    Task<int> ProcessRegistration(RegistrationId registrationId);
}
