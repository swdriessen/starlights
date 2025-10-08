using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Services.Processing;

public class RegistrationProcessContext
{
    private readonly IPersistence _persistence;

    public RegistrationProcessContext(Registration registration, IPersistence persistence)
    {
        Registration = registration;
        _persistence = persistence;
    }

    /// <summary>
    /// The registration that is currently being processed. Itself it already persisted.
    /// </summary>
    public Registration Registration { get; }

    /// <summary>
    /// A list of new registrations that were created during the processing of the current registration.
    /// </summary>
    public List<Registration> NewRegistrations { get; } = [];

    /// <summary>
    /// Gets a repository for the current persistence context.
    /// </summary>
    public T GetRepository<T>() where T : IRepository
    {
        return _persistence.GetRepository<T>();
    }
}

