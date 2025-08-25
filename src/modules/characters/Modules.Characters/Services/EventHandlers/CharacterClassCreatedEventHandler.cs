using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Classes.Eventing;
using Starlights.Platform.Data;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Services.EventHandlers;
public sealed class CharacterClassCreatedEventHandler : IDomainEventHandler<ClassComponentChangedEvent>
{
    private readonly ILogger<CharacterClassCreatedEventHandler> _logger;
    private readonly IPersistence _persistence;

    public CharacterClassCreatedEventHandler(ILogger<CharacterClassCreatedEventHandler> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public async Task HandleAsync(ClassComponentChangedEvent domainEvent)
    {
        _logger.LogWarning("Character class created event handled for character ID: {CharacterId}", domainEvent.CharacterId);

        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(domainEvent.CharacterId);
        if (character is null)
        {
            _logger.LogError("Character with ID {CharacterId} not found.", domainEvent.CharacterId);
            return;
        }

        var component = character.GetRequiredComponent<ClassComponent>();

        if (component.Classes.Count == 0)
        {
            _logger.LogWarning("Character {CharacterId} has no classes.", domainEvent.CharacterId);
            return;
        }

        var names = component.Classes.Select(x => x.Name);
        _logger.LogInformation("Character {CharacterId} has classes: {ClassNames}", domainEvent.CharacterId, string.Join(", ", names));

        await Task.CompletedTask;
    }
}
