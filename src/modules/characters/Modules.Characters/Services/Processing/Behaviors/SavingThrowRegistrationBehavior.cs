using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Elements.Integration;
using Starlights.Modules.Elements.Integration.Models;

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

        using var _ = CharactersInstrumentation.StartActivity("Saving Throw Registration Behavior");

        var associatedElement = await _elements.GetSavingThrowModel(newRegistration.AssociatedElementId) ?? throw new InvalidOperationException($"Saving Throw with ID {newRegistration.AssociatedElementId} not found.");

        var characters = context.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(newRegistration.CharacterId) ?? throw new InvalidOperationException($"Character with ID {newRegistration.CharacterId} not found.");

        var primaryScore = await GetPrimaryAbilityScore(context, character, associatedElement);

        if (primaryScore is null)
        {
            _logger.LogInformation("Creating saving throw '{SavingThrowName}' without primary ability score for character '{CharacterId}'", associatedElement.Name, character.Id.Value);
            character.CreateSavingThrowWithoutAbilityScore(newRegistration.Id, associatedElement.Name);
        }
        else
        {
            _logger.LogInformation("Creating saving throw '{SavingThrowName}' with primary ability score '{AbilityScoreName}' for character '{CharacterId}'", associatedElement.Name, primaryScore.Name, character.Id.Value);
            character.CreateSavingThrow(newRegistration.Id, associatedElement.Name, primaryScore.Id, primaryScore.Abbreviation);
        }
    }

    private static async Task<AbilityScore?> GetPrimaryAbilityScore(RegistrationProcessContext context, Character character, SavingThrowDataModel save)
    {
        // first check if the save is already associated with a new registration
        foreach (var registration in context.NewRegistrations)
        {
            if (registration.AssociatedElementId == save.PrimaryAbilityElementId)
            {
                return character.AbilityScores.SingleOrDefault(a => a.AssociatedRegistrationId == registration.Id);
            }
        }

        // otherwise, we need to fetch the registrations from the repository
        var registrations = context.GetRepository<IRegistrationRepository>();

        var existingRegistrations = await registrations.GetRegistrationsByAssociationsAsync(character.Id, new ElementId(save.PrimaryAbilityElementId));

        foreach (var abilityRegistration in existingRegistrations)
        {
            if (abilityRegistration.AssociatedElementId == save.PrimaryAbilityElementId)
            {
                return character.AbilityScores.SingleOrDefault(a => a.AssociatedRegistrationId == abilityRegistration.Id);
            }
        }

        return null;
    }
}