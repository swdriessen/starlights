using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Components;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.SavingThrows.Eventing;

namespace Starlights.Modules.Characters.Domain.SavingThrows;

public sealed class SavingThrowsComponent : CharacterComponentBase
{
    private readonly List<SavingThrow> _savingThrows = [];

    private SavingThrowsComponent(CharacterId parentCharacter)
        : base(parentCharacter)
    {
    }

    /// <summary>
    /// Gets the collection of saving throws associated with the parent character.
    /// </summary>
    public IReadOnlyCollection<SavingThrow> SavingThrows => _savingThrows.AsReadOnly();

    /// <summary>
    /// Updates the ability score modifier for all saving throws associated with the specified ability score.
    /// </summary>
    public void UpdateAbilityScoreModifier(AbilityScoreId abilityScoreId, int modifier)
    {
        foreach (var savingThrow in _savingThrows.Where(s => s.AbilityScoreId == abilityScoreId))
        {
            if (savingThrow.UpdateAbilityScoreModifier(modifier))
            {
                AddDomainEvent(new SavingThrowUpdatedEvent() { CharacterId = ParentCharacter, SavingThrowId = savingThrow.Id });
            }
        }
    }

    /// <summary>
    /// Creates a new saving throw associated with the specified registration and ability score.
    /// </summary>
    public SavingThrow CreateSavingThrow(RegistrationId associatedRegistrationId, string name, AbilityScoreId abilityScoreId, string abilityScoreAbbreviation, double sortingOrder = 0)
    {
        var newSavingThrow = SavingThrow.Create(associatedRegistrationId, name, abilityScoreId, abilityScoreAbbreviation, sortingOrder);
        _savingThrows.Add(newSavingThrow);
        AddDomainEvent(new SavingThrowCreatedEvent { CharacterId = ParentCharacter, SavingThrowId = newSavingThrow.Id });
        return newSavingThrow;
    }

    /// <summary>
    /// Creates a new instance of the SavingThrowsComponent for the specified character.
    /// </summary>
    public static SavingThrowsComponent Create(CharacterId parentCharacter)
    {
        return new SavingThrowsComponent(parentCharacter);
    }
}