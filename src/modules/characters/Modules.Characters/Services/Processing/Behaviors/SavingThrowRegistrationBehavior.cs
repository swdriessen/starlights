using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.SavingThrows;
using Starlights.Modules.Elements.Integration;

namespace Starlights.Modules.Characters.Services.Processing.Behaviors;

/// <summary>
/// This behavior is responsible for creating a saving throw when a new saving throw registration is created.
/// </summary>
public sealed class SavingThrowRegistrationBehavior : IRegistrationBehavior
{
    private readonly ILogger<SavingThrowRegistrationBehavior> _logger;
    private readonly IElementsModuleQueries _elements;

    public SavingThrowRegistrationBehavior(ILogger<SavingThrowRegistrationBehavior> logger, IElementsModuleQueries elements)
    {
        _logger = logger;
        _elements = elements;
    }

    public async Task Registered(Registration newRegistration, RegistrationProcessContext context)
    {
        if (newRegistration.AssociatedElementType != "Saving Throw")
        {
            return;
        }

        // a new saving throw registration was created, we need to create the saving throw in the character
        var characters = context.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(newRegistration.CharacterId) ?? throw new InvalidOperationException($"Character with ID {newRegistration.CharacterId} not found.");

        // fetch the associated saving throw element
        var associatedElement = await _elements.GetSavingThrowModel(newRegistration.AssociatedElementId) ?? throw new InvalidOperationException($"Saving Throw with ID {newRegistration.AssociatedElementId} not found.");

        // try to find the primary ability score for this saving throw
        var primaryScore = await GetPrimaryAbilityScore(context, character, new(associatedElement.PrimaryAbilityElementId));
        if (primaryScore is null)
        {
            _logger.LogError("Could not find primary ability score for saving throw '{SavingThrowName}' [character='{CharacterId}']", associatedElement.Name, character.Id.Value);
            return;
        }

        character.UpdateComponent<SavingThrowsComponent>((component, _) =>
        {
            // create the saving throw in the character
            component.CreateSavingThrow(newRegistration.Id, associatedElement.Name, primaryScore.Id, primaryScore.Abbreviation);
        });
    }

    private static async Task<AbilityScore?> GetPrimaryAbilityScore(RegistrationProcessContext context, Character character, ElementId primaryAbilityElementId)
    {
        var component = character.GetRequiredComponent<AbilitiesComponent>();

        // if the ability score was registered in the context of the current processing registration
        foreach (var registration in context.NewRegistrations.Where(r => r.AssociatedElementId == primaryAbilityElementId))
        {
            return component.AbilityScores.SingleOrDefault(a => a.AssociatedRegistrationId == registration.Id);
        }

        // otherwise, we need to fetch the registrations from the repository        
        var existingRegistrations = await context.GetRepository<IRegistrationRepository>()
            .GetRegistrationsByAssociationsAsync(character.Id, primaryAbilityElementId);

        foreach (var registration in existingRegistrations.Where(r => r.AssociatedElementId == primaryAbilityElementId))
        {
            return component.AbilityScores.SingleOrDefault(a => a.AssociatedRegistrationId == registration.Id);
        }

        return null;
    }

    public Task Unregister(Registration existingRegistration)
    {
        if (existingRegistration.AssociatedElementType != "Saving Throw")
        {
            return Task.CompletedTask;
        }

        throw new NotImplementedException();
    }
}