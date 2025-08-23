using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Classes.Eventing;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Elements.Integration;

namespace Starlights.Modules.Characters.Services.Processing.Behaviors;

/// <summary>
/// This behavior is responsible for creating a character class when a new class registration is created.
/// </summary>
public sealed class ClassRegistrationBehavior : IRegistrationBehavior
{
    private readonly ILogger<ClassRegistrationBehavior> _logger;
    private readonly IElementsModuleQueries _elements;

    public ClassRegistrationBehavior(ILogger<ClassRegistrationBehavior> logger, IElementsModuleQueries elements)
    {
        _logger = logger;
        _elements = elements;
    }

    public async Task Registered(Registration newRegistration, RegistrationProcessContext context)
    {
        if (newRegistration.AssociatedElementType != "Class")
        {
            return;
        }

        using var _ = CharactersInstrumentation.StartActivity($"Class Registration Behavior | {newRegistration.AssociatedElementName}");

        // when a new class element is registered, we need to create the character class for the character
        var associatedElement = await _elements.GetElementWithRules(newRegistration.AssociatedElementId) ?? throw new InvalidOperationException($"Class with ID {newRegistration.AssociatedElementId} not found.");

        // get the character (could be a property in the context, specially if we need character level and other data later for requirements)
        var characters = context.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(newRegistration.CharacterId) ?? throw new InvalidOperationException($"Character with ID {newRegistration.CharacterId} not found.");

        _logger.LogInformation("Creating class '{ClassName}' for character '{CharacterId}'", associatedElement.Name, character.Id.Value);

        character.WithComponent<ClassComponent, ProgressionComponent>((classComponent, progressionComponent, events) =>
        {
            var newClass = classComponent.AddClass(newRegistration.Id, associatedElement.Name);
            events.RecordEvent(new CharacterClassCreatedEvent() { CharacterId = character.Id, ClassId = newClass.Id });

            var newCharacterLevel = classComponent.Classes.Sum(x => x.Level);
            progressionComponent.SetCharacterLevel(newCharacterLevel);
        });

        // persisting happens in the unit of work after all behaviors are processed
    }
}
