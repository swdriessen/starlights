using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;
using Starlights.Platform.Eventing;

namespace Modules.Characters.Services.EventHandlers;

public sealed class AbilityRegistrationCreatedEventHandler : IDomainEventHandler<AbilityRegistrationCreatedEvent>
{
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;

    public AbilityRegistrationCreatedEventHandler(IPersistence persistence, IElementsModuleQueries elements)
    {
        _persistence = persistence;
        _elements = elements;
    }

    public async Task HandleAsync(AbilityRegistrationCreatedEvent raisedEvent)
    {
        using var _ = CharactersInstrumentation.StartActivity($"{nameof(AbilityRegistrationCreatedEventHandler)} | {raisedEvent.AssociatedElementName} ({raisedEvent.AssociatedElementType})");

        // get the registration
        var registrations = _persistence.GetRepository<IRegistrationRepository>();
        var registration = await registrations.GetRegistrationAsync(new(raisedEvent.RegistrationId)) ?? throw new InvalidOperationException($"Registration with ID {raisedEvent.RegistrationId} not found.");

        // get character
        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(raisedEvent.CharacterId) ?? throw new InvalidOperationException($"Character with ID {raisedEvent.CharacterId} not found.");

        // get the associated ability element
        var associatedElement = await _elements.GetAbilityModel(registration.AssociatedElementId);

        // create ability score based on the element data
        character.CreateAbilityScore(registration.Id, associatedElement.Name, associatedElement.Abbreviation);

        await _persistence.SaveChangesAsync();
    }
}