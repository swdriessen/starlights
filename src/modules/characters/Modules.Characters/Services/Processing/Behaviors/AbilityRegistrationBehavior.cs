using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Elements.Integration;

namespace Modules.Characters.Services.Processing.Behaviors;

/// <summary>
/// This behavior is responsible for creating an ability score when a new ability registration is created.
/// </summary>
public sealed class AbilityRegistrationBehavior : IRegistrationBehavior
{
    private readonly IElementsModuleQueries _elements;

    public AbilityRegistrationBehavior(IElementsModuleQueries elements)
    {
        _elements = elements;
    }

    public async Task Registered(Registration newRegistration, RegistrationProcessContext context)
    {
        if (newRegistration.AssociatedElementType == "Ability")
        {
            using var _ = CharactersInstrumentation.StartActivity("Ability Registration Behavior");

            // when a new ability element is registered, we need to create the ability score for the character
            var associatedElement = await _elements.GetAbilityModel(newRegistration.AssociatedElementId) ?? throw new InvalidOperationException($"Ability with ID {newRegistration.AssociatedElementId} not found.");

            // get the character (could be a property in the context, specially if we need character level and other data later for requirements)
            var characters = context.GetRepository<ICharactersRepository>();
            var character = await characters.GetCharacterAsync(newRegistration.CharacterId) ?? throw new InvalidOperationException($"Character with ID {newRegistration.CharacterId} not found.");

            character.CreateAbilityScore(newRegistration.Id, associatedElement.Name, associatedElement.Abbreviation);
        }
    }
}
