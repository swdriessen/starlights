using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.Skills;
using Starlights.Modules.Elements.Integration;
using Starlights.Modules.Elements.Integration.Models;

namespace Starlights.Modules.Characters.Services.Processing.Behaviors;

/// <summary>
/// This behavior is responsible for creating a skill when a new skill registration is created.
/// </summary>
public sealed class SkillRegistrationBehavior : IRegistrationBehavior
{
    private readonly ILogger<SkillRegistrationBehavior> _logger;
    private readonly IElementsModuleQueries _elements;

    public SkillRegistrationBehavior(ILogger<SkillRegistrationBehavior> logger, IElementsModuleQueries elements)
    {
        _logger = logger;
        _elements = elements;
    }

    public async Task Registered(Registration newRegistration, RegistrationProcessContext context)
    {
        if (newRegistration.AssociatedElementType != "Skill")
        {
            return;
        }

        using var _ = CharactersInstrumentation.StartActivity("Skill Registration Behavior");

        var associatedElement = await _elements.GetSkillModel(newRegistration.AssociatedElementId) ?? throw new InvalidOperationException($"Skill with ID {newRegistration.AssociatedElementId} not found.");

        var characters = context.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(newRegistration.CharacterId) ?? throw new InvalidOperationException($"Character with ID {newRegistration.CharacterId} not found.");

        var skillsComponent = character.GetRequiredComponent<SkillsComponent>();

        var primaryScore = await GetPrimaryAbilityScore(context, character, associatedElement);

        if (primaryScore is null)
        {
            _logger.LogWarning("Creating skill '{SkillName}' without primary ability score [character='{CharacterId}']", associatedElement.Name, character.Id.Value);
            skillsComponent.CreateSkillWithoutAbilityScore(newRegistration.Id, associatedElement.Name);
        }
        else
        {
            _logger.LogInformation("Creating skill '{SkillName}' with primary ability score '{AbilityScoreName}' [character='{CharacterId}']", associatedElement.Name, primaryScore.Name, character.Id.Value);
            skillsComponent.CreateSkill(newRegistration.Id, associatedElement.Name, primaryScore.Id, primaryScore.Abbreviation);
        }
    }

    private static async Task<AbilityScore?> GetPrimaryAbilityScore(RegistrationProcessContext context, Character character, SkillDataModel skill)
    {
        var abilities = character.GetRequiredComponent<AbilitiesComponent>();

        // first check if the skill is already associated with a new registration
        foreach (var registration in context.NewRegistrations)
        {
            if (registration.AssociatedElementId == skill.PrimaryAbilityElementId)
            {
                return abilities.AbilityScores.SingleOrDefault(a => a.AssociatedRegistrationId == registration.Id);
            }
        }

        // otherwise, we need to fetch the registrations from the repository
        var registrations = context.GetRepository<IRegistrationRepository>();

        var existingRegistrations = await registrations.GetRegistrationsByAssociationsAsync(character.Id, new ElementId(skill.PrimaryAbilityElementId));

        foreach (var abilityRegistration in existingRegistrations)
        {
            if (abilityRegistration.AssociatedElementId == skill.PrimaryAbilityElementId)
            {
                return abilities.AbilityScores.SingleOrDefault(a => a.AssociatedRegistrationId == abilityRegistration.Id);
            }
        }

        return null;
    }
}
