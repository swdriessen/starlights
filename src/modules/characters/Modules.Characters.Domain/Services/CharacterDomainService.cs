using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Classes.Eventing;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.Progression.Eventing;
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
        _logger.LogInformation("Creating class '{ClassName}' [character='{CharacterId}']", className, character.Id.Value);

        character.UpdateComponents<ClassComponent, ProgressionComponent>((classes, progression, events) =>
        {
            var newClass = classes.CreateClass(newRegistration.Id, className, events);
            events.AddDomainEvent(new CharacterClassCreatedEvent { CharacterId = character.Id, ClassId = newClass.Id });

            progression.SetCharacterLevel(classes.GetAggregatedLevel());
            events.AddDomainEvent(new CharacterLevelChangedEvent { CharacterId = character.Id, NewLevel = progression.CharacterLevel });
        });
    }
}
