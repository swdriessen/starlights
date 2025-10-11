using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Services.Processing;

public class RegistrationProcessingContext
{
    private readonly IPersistence _persistence;

    public RegistrationProcessingContext(Registration registration, Character character, IPersistence persistence)
    {
        Registration = registration;
        Character = character;
        _persistence = persistence;
    }

    /// <summary>
    /// The registration that is currently being processed. Itself it already persisted.
    /// </summary>
    public Registration Registration { get; }

    /// <summary>
    /// Gets the character associated with this registration.
    /// </summary>
    public Character Character { get; }

    /// <summary>
    /// A list of new registrations that were created during the processing of the current registration.
    /// </summary>
    public List<Registration> NewRegistrations { get; } = [];

    /// <summary>
    /// Computes the applicable progression level for a given registration, considering its progression origin if any.
    /// Falls back to character level if no specific origin can be resolved.
    /// </summary>
    public int GetProgressionLevel(Registration originRegistration)
    {
        var progressionOrigin = originRegistration.GetProgressionOriginForChild();


        // If a progression origin is already set, try to resolve level from it (e.g., class level)
        if (progressionOrigin is RegistrationId)
        {
            // Try resolve against character components we have in-memory, avoid repository lookups
            // ClassComponent: find the class with matching registration            
            var classComponent = Character.GetRequiredComponent<ClassComponent>();
            var originClass = classComponent.Classes.SingleOrDefault(c => c.Registration == progressionOrigin);
            if (originClass is not null)
            {
                return originClass.Level;
            }

            // Future origins (e.g., Item) could be added here by probing their components by registration id
            // If not found, fall back to character level
        }

        return Character.GetRequiredComponent<ProgressionComponent>().CharacterLevel;
    }

    /// <summary>
    /// Gets a repository for the current persistence context.
    /// </summary>
    public T GetRepository<T>() where T : IRepository
    {
        return _persistence.GetRepository<T>();
    }
}

