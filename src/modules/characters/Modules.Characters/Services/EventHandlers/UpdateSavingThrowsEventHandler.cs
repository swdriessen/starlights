using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.SavingThrows;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Services.EventHandlers;

/// <summary>
/// Event handler that updates saving throws when an ability score is updated.
/// </summary>
public sealed class UpdateSavingThrowsEventHandler : IDomainEventHandler<AbilityScoreUpdatedEvent>
{
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;

    public UpdateSavingThrowsEventHandler(IPersistence persistence, IElementsModuleQueries elements)
    {
        _persistence = persistence;
        _elements = elements;
    }

    public async Task HandleAsync(AbilityScoreUpdatedEvent domainEvent)
    {
        using var _ = CharactersInstrumentation.StartActivity($"{nameof(UpdateSavingThrowsEventHandler)} | score={domainEvent.NewAbilityScoreValue} mod={domainEvent.NewAbilityModifier}");

        var characters = _persistence.GetRepository<ICharactersRepository>();

        var character = await characters.GetCharacterAsync(domainEvent.CharacterId);
        if (character is null)
        {
            return;
        }

        character.UpdateComponent<SavingThrowsComponent>((component, _) =>
            component.UpdateAbilityScoreModifier(domainEvent.AbilityScoreId, domainEvent.NewAbilityModifier));

        await _persistence.SaveChangesAsync();
    }
}
