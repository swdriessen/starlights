using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Elements.Integration.Models;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Services.Processing;

public class ProcessingContext
{
    private readonly IPersistence _persistence;

    public ProcessingContext(Registration registration, Character character, IPersistence persistence)
    {
        Registration = registration;
        Character = character;
        _persistence = persistence;
    }

    /// <summary>
    /// The persisted registration that is currently being processed.
    /// </summary>
    public Registration Registration { get; }

    /// <summary>
    /// Gets the character associated with this registration.
    /// </summary>
    public Character Character { get; }

    /// <summary>
    /// Gets or sets a key/value collection that can be used to share data within the scope of this processing request.
    /// </summary>
    public IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();

    /// <summary>
    /// Gets a repository for the current persistence context.
    /// </summary>
    public T GetRepository<T>() where T : IRepository
    {
        return _persistence.GetRepository<T>();
    }

    /// <summary>
    /// Computes the applicable progression level for a given registration, considering its progression origin if any.
    /// Falls back to character level if no specific origin can be resolved.
    /// </summary>
    public int GetProgressionLevel(Registration originRegistration)
    {
        var progressionCapableTypes = new[] { "class" };

        var progressionOrigin = GetProgressionOrigin(originRegistration, progressionCapableTypes);
        if (progressionOrigin is RegistrationId id)
        {
            // initially only a class origin is supported for progression of elements
            var originClass = Character.GetRequiredComponent<ClassComponent>()
                .GetClassByRegistration(id);

            if (originClass is not null)
            {
                return originClass.Level;
            }

            // If not found, fall back to character level
        }

        return Character.GetRequiredComponent<ProgressionComponent>().CharacterLevel;

        static RegistrationId? GetProgressionOrigin(Registration registration, params string[] progressionCapableTypes)
        {
            if (registration.ProgressionOriginRegistrationId is RegistrationId progressionOrigin)
            {
                return progressionOrigin;
            }

            if (progressionCapableTypes.Contains(registration.AssociatedElementType, StringComparer.OrdinalIgnoreCase))
            {
                return registration.Id;
            }

            return null;
        }
    }
}


// experimental extensions for strongly typed access to common items in the processing context
// in .NET 10 prefer extension properties
public static class ProcessingContextExtensions
{
    public static T GetRequiredItem<T>(this ProcessingContext context, object key)
    {
        if (context.Items.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        throw new KeyNotFoundException($"The required item with key '{key}' was not found in the processing context.");
    }
    public static void SetItem<T>(this ProcessingContext context, object key, T value)
    {
        context.Items[key] = value;
    }

    public static ElementDataModel GetAssociatedElement(this ProcessingContext context)
    {
        return context.GetRequiredItem<ElementDataModel>("AssociatedElement");
    }

    public static void SetAssociatedElement(this ProcessingContext context, ElementDataModel associatedElement)
    {
        context.SetItem("AssociatedElement", associatedElement);
    }

    public static void SetCharacterRegistations(this ProcessingContext context, List<Registration> registrations)
    {
        context.SetItem("CharacterRegistrations", registrations);
    }

    public static List<Registration> GetCharacterRegistrations(this ProcessingContext context)
    {
        return context.GetRequiredItem<List<Registration>>("CharacterRegistrations");
    }
}