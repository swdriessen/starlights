using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Domain.Services;

public sealed class ClassManagementService
{
    private readonly ILogger<ClassManagementService> _logger;

    public ClassManagementService(ILogger<ClassManagementService> logger)
    {
        _logger = logger;
    }

    public void AddCharacterClass(Character character, Registration newRegistration, string className)
    {
        character.UpdateComponents<ClassComponent, ProgressionComponent>((classes, progression, _) =>
        {
            _logger.LogInformation("creating class '{ClassName}' [CharacterId='{CharacterId}']", className, character.Id.Value);

            classes.CreateClass(newRegistration.Id, className);

            var newLevel = classes.CalculateCharacterLevel();

            progression.SetCharacterLevel(newLevel);

            _logger.LogInformation("created new class '{ClassName}' [CharacterId='{CharacterId}', CharacterLevel={CharacterLevel}]", className, character.Id.Value, newLevel);
        });
    }

    public void RemoveCharacterClass(Character character, Registration existingRegistration)
    {
        character.UpdateComponents<ClassComponent, ProgressionComponent>((classes, progression, _) =>
        {
            _logger.LogInformation("removing class for registration '{RegistrationId}' [CharacterId='{CharacterId}']", existingRegistration.Id.Value, character.Id.Value);

            classes.RemoveClass(existingRegistration.Id);

            var newLevel = classes.CalculateCharacterLevel();

            progression.SetCharacterLevel(newLevel);

            _logger.LogInformation("removed class for registration '{RegistrationId}' [CharacterId='{CharacterId}', CharacterLevel={CharacterLevel}]", existingRegistration.Id.Value, character.Id.Value, newLevel);
        });
    }
}
