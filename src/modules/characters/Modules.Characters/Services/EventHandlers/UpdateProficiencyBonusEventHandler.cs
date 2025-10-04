using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.Progression.Eventing;
using Starlights.Platform.Data;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Services.EventHandlers;

public sealed class UpdateProficiencyBonusEventHandler : IDomainEventHandler<CharacterLevelChangedEvent>
{
    private readonly ILogger<UpdateProficiencyBonusEventHandler> _logger;
    private readonly IPersistence _persistence;

    public UpdateProficiencyBonusEventHandler(ILogger<UpdateProficiencyBonusEventHandler> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public async Task HandleAsync(CharacterLevelChangedEvent domainEvent)
    {
        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(domainEvent.CharacterId);
        if (character is null)
        {
            return;
        }

        character.UpdateComponent<ProficiencyComponent>((component, _) =>
        {
            var value = domainEvent.NewLevel switch
            {
                <= 4 => 2,
                <= 8 => 3,
                <= 12 => 4,
                <= 16 => 5,
                _ => 6
            };

            _logger.LogInformation(value >= component.ProficiencyBonus
                ? "Increasing proficiency bonus to {ProficiencyBonus} for character {CharacterId}"
                : "Decreasing proficiency bonus to {ProficiencyBonus} for character {CharacterId}",
                value, domainEvent.CharacterId);
            component.SetProficiencyBonus(value);
        });

        await _persistence.SaveChangesAsync();
    }
}