using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Modules.Characters.Services.Processing;

public class RegistrationProcessContext
{
    public RegistrationProcessContext(IRegistrationRepository repository, Registration registration)
    {
        Repository = repository;
        Registration = registration;
    }

    public IRegistrationRepository Repository { get; }
    public Registration Registration { get; }
}
