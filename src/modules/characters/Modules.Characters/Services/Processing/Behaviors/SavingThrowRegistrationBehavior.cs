using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.SavingThrows;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Services.Processing.Behaviors;

/// <summary>
/// This behavior is responsible for creating a saving throw when a new saving throw registration is created.
/// </summary>
public sealed class SavingThrowRegistrationBehavior : IRegistrationBehavior
{
    private readonly ILogger<SavingThrowRegistrationBehavior> _logger;
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;

    public SavingThrowRegistrationBehavior(ILogger<SavingThrowRegistrationBehavior> logger, IPersistence persistence, IElementsModuleQueries elements)
    {
        _logger = logger;
        _persistence = persistence;
        _elements = elements;
    }

    public async Task Registered(Registration newRegistration)
    {
        if (newRegistration.AssociatedElementType != "Saving Throw")
        {
            return;
        }
        using var _ = CharactersInstrumentation.StartActivity($"{nameof(SavingThrowRegistrationBehavior)} | {newRegistration.AssociatedElementName}");

        // a new saving throw registration was created, we need to create the saving throw in the character
        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(newRegistration.CharacterId) ?? throw new InvalidOperationException($"Character with ID {newRegistration.CharacterId} not found.");

        // fetch the associated saving throw element
        var associatedElement = await _elements.GetSavingThrowModel(newRegistration.AssociatedElementId) ?? throw new InvalidOperationException($"Saving Throw with ID {newRegistration.AssociatedElementId} not found.");

        // try to find the primary ability score for this saving throw
        var primaryScore = await GetPrimaryAbilityScore(character, new(associatedElement.PrimaryAbilityElementId));
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

    private async Task<AbilityScore?> GetPrimaryAbilityScore(Character character, ElementId primaryAbilityElementId)
    {
        var component = character.GetRequiredComponent<AbilitiesComponent>();

        // otherwise, we need to fetch the registrations from the repository        
        var existingRegistrations = await _persistence.GetRepository<IRegistrationRepository>()
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