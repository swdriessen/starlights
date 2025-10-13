using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.Services;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Services.Processing.Behaviors;

/// <summary>
/// This behavior is responsible for creating a character class when a new class registration is created.
/// </summary>
public sealed class ClassRegistrationBehavior : IRegistrationBehavior
{
    private readonly IElementsModuleQueries _elements;
    private readonly IPersistence _persistence;
    private readonly ClassManagementService _service;

    public ClassRegistrationBehavior(IElementsModuleQueries elements, IPersistence persistence, ClassManagementService service)
    {
        _elements = elements;
        _persistence = persistence;
        _service = service;
    }

    public async Task Registered(Registration newRegistration)
    {
        if (newRegistration.AssociatedElementType != "Class")
        {
            return;
        }

        using var _ = CharactersInstrumentation.StartActivity($"{nameof(ClassRegistrationBehavior)} | {newRegistration.AssociatedElementName}");

        // when a new class element is registered, we need to create the character class for the character
        var associatedElement = await _elements.GetElementWithRules(newRegistration.AssociatedElementId) ?? throw new InvalidOperationException($"Class with ID {newRegistration.AssociatedElementId} not found.");

        // get the character (could be a property in the context, specially if we need character level and other data later for requirements)
        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(newRegistration.CharacterId) ?? throw new InvalidOperationException($"Character with ID {newRegistration.CharacterId} not found.");

        _service.AddCharacterClass(character, newRegistration, associatedElement.Name);
    }

    public async Task Unregister(Registration existingRegistration)
    {
        if (existingRegistration.AssociatedElementType != "Class")
        {
            return;
        }

        using var _ = CharactersInstrumentation.StartActivity($"Class Unregistration Behavior | Unregistered {existingRegistration.AssociatedElementName}");

        // when a class registration is removed, we need to remove the character class from the character

        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(existingRegistration.CharacterId) ?? throw new InvalidOperationException($"Character with ID {existingRegistration.CharacterId} not found.");

        _service.RemoveCharacterClass(character, existingRegistration);
    }
}
