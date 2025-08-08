using Microsoft.Extensions.DependencyInjection;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;
using Starlights.Platform.Eventing;

namespace Modules.Characters.Services.EventHandlers;

public sealed class CreateAbilityScoreEventHandler : IDomainEventHandler<RegistrationCreated>
{
    private readonly IServiceScopeFactory _factory;

    public CreateAbilityScoreEventHandler(IServiceScopeFactory factory)
    {
        _factory = factory;
    }

    public async Task HandleAsync(RegistrationCreated raisedEvent)
    {
        if (raisedEvent.AssociatedElementType != "Ability") // TODO: create 'AbilityRegistered' event type      
        {
            return;
        }

        using var _ = CharactersInstrumentation.StartActivity($"{nameof(CreateAbilityScoreEventHandler)} | {raisedEvent.AssociatedElementName} ({raisedEvent.AssociatedElementType})");

        using var scope = _factory.CreateScope();
        {
            using var persistence = scope.ServiceProvider.GetRequiredService<IPersistence>();

            // get the registration
            var registrations = persistence.GetRepository<IRegistrationRepository>();
            var registration = await registrations.GetRegistrationAsync(new(raisedEvent.RegistrationId)) ?? throw new InvalidOperationException($"Registration with ID {raisedEvent.RegistrationId} not found.");

            // get character
            var characters = persistence.GetRepository<ICharactersRepository>();
            var character = await characters.GetCharacterAsync(raisedEvent.CharacterId) ?? throw new InvalidOperationException($"Character with ID {raisedEvent.CharacterId} not found.");

            // get the associated ability element
            var elements = scope.ServiceProvider.GetRequiredService<IElementsModuleQueries>();
            var associatedElement = await elements.GetAbilityModel(registration.AssociatedElementId);

            // create ability score based on the element data
            character.CreateAbilityScore(registration.Id, associatedElement.Name, associatedElement.Abbreviation);

            await persistence.SaveChangesAsync();
        }
    }
}