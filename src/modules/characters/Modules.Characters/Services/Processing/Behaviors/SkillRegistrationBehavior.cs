using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Elements.Integration;
using Starlights.Modules.Elements.Integration.Models;

namespace Modules.Characters.Services.Processing.Behaviors;

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
        if (newRegistration.AssociatedElementType == "Skill")
        {
            using var _ = CharactersInstrumentation.StartActivity("Skill Registration Behavior");

            var associatedElement = await _elements.GetSkillModel(newRegistration.AssociatedElementId) ?? throw new InvalidOperationException($"Skill with ID {newRegistration.AssociatedElementId} not found.");

            var characters = context.GetRepository<ICharactersRepository>();
            var character = await characters.GetCharacterAsync(newRegistration.CharacterId) ?? throw new InvalidOperationException($"Character with ID {newRegistration.CharacterId} not found.");

            var primaryScore = await GetPrimaryAbilityScore(context.GetRepository<IRegistrationRepository>(), character, associatedElement);

            if (primaryScore is null)
            {
                _logger.LogInformation("Creating skill '{SkillName}' without primary ability score for character '{CharacterId}'", associatedElement.Name, character.Id.Value);
                character.CreateSkillWithoutAbilityScore(newRegistration.Id, associatedElement.Name);
            }
            else
            {
                _logger.LogInformation("Creating skill '{SkillName}' with primary ability score '{AbilityScoreName}' for character '{CharacterId}'", associatedElement.Name, primaryScore.Name, character.Id.Value);
                character.CreateSkill(newRegistration.Id, associatedElement.Name, primaryScore.Id, primaryScore.Abbreviation);
            }
        }
    }

    private static async Task<AbilityScore?> GetPrimaryAbilityScore(IRegistrationRepository registrations, Character character, SkillDataModel skill)
    {
        var all = await registrations.GetRegistrationsByAssociationsAsync(character.Id, new ElementId(skill.PrimaryAbilityElementId));

        //var all = await registrations.GetRegistrationsAsync(character.Id);

        foreach (var abilityRegistration in all)
        {
            if (abilityRegistration.AssociatedElementId == skill.PrimaryAbilityElementId)
            {
                return character.AbilityScores.SingleOrDefault(a => a.AssociatedRegistrationId == abilityRegistration.Id);
            }
        }

        return null;
    }
}
