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

    public IReadOnlyCollection<SavingThrow> SavingThrows => _savingThrows.AsReadOnly();



    public SavingThrow CreateSavingThrow(RegistrationId associatedRegistrationId, string name, AbilityScoreId abilityScoreId, string abilityScoreAbbreviation)
    {
        var save = SavingThrow.Create(associatedRegistrationId, name, abilityScoreId, abilityScoreAbbreviation);
        _savingThrows.Add(save);
        AddDomainEvent(new SavingThrowCreatedEvent { CharacterId = ParentCharacter, SavingThrowId = save.Id });
        return save;
    }

    public SavingThrow CreateSavingThrowWithoutAbilityScore(RegistrationId associatedRegistrationId, string name)
    {
        var save = SavingThrow.CreateWithoutAbilityScore(associatedRegistrationId, name);
        _savingThrows.Add(save);
        AddDomainEvent(new SavingThrowCreatedEvent { CharacterId = ParentCharacter, SavingThrowId = save.Id });
        return save;
    }

    public void UpdateAbilityScoreModifier(AbilityScoreId abilityScoreId, int modifier)
    {
        foreach (var savingThrow in _savingThrows.Where(s => s.AbilityScoreId == abilityScoreId))
        {
            savingThrow.UpdateAbilityScoreModifier(modifier);
            AddDomainEvent(new SavingThrowUpdatedEvent() { CharacterId = ParentCharacter, SavingThrowId = savingThrow.Id });
        }
    }

    public static SavingThrowsComponent Create(CharacterId parentCharacter)
    {
        return new SavingThrowsComponent(parentCharacter);
    }
}