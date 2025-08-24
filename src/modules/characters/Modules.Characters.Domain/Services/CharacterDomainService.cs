using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Domain.Services;

public sealed class CharacterDomainService
{
    private readonly ILogger<CharacterDomainService> _logger;

    public CharacterDomainService(ILogger<CharacterDomainService> logger)
    {
        _logger = logger;
    }

    public void AddCharacterClass(Character character, Registration newRegistration, string className)
    {
        _logger.LogInformation("Creating class '{ClassName}' for character '{CharacterId}'", className, character.Id.Value);

        character.UpdateComponents<ClassComponent, ProgressionComponent>((classComponent, progressionComponent, events) =>
        {
            classComponent.AddClass(newRegistration.Id, className, events);
            progressionComponent.SetCharacterLevel(classComponent.GetCombinedLevel());
        });
    }
}
