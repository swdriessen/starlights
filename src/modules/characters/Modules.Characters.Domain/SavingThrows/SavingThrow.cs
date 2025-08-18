using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Domain;

namespace Starlights.Modules.Characters.Domain.SavingThrows;

/// <summary>
/// Represents a saving throw for a character (e.g. Strength Saving Throw) which is associated with a primary ability.
/// </summary>
public sealed class SavingThrow : EntityBase<SavingThrowId>
{
    public SavingThrow(RegistrationId associatedRegistrationId, string name, AbilityScoreId abilityScoreId, string? abilityScoreAbbreviation)
        : base(SavingThrowId.New())
    {
        AssociatedRegistrationId = associatedRegistrationId;
        Name = name;
        AbilityScoreId = abilityScoreId;
        AbilityScoreAbbreviation = abilityScoreAbbreviation;
        Recalculate();
    }

    public RegistrationId AssociatedRegistrationId { get; }
    public AbilityScoreId AbilityScoreId { get; private set; }
    public string Name { get; }
    public string? AbilityScoreAbbreviation { get; private set; }
    public int AbilityScoreModifier { get; private set; }
    public int AdditionalBonus { get; private set; }
    public int CalculatedBonus { get; private set; }

    private void Recalculate() => CalculatedBonus = AbilityScoreModifier + AdditionalBonus;

    public void UpdateAbilityScoreModifier(int modifier)
    {
        AbilityScoreModifier = modifier;
        Recalculate();
    }

    public void UpdateAdditionalBonus(int bonus)
    {
        AdditionalBonus = bonus;
        Recalculate();
    }

    public void WithAbilityScore(AbilityScoreId abilityScoreId, string abilityScoreAbbreviation)
    {
        if (abilityScoreId == default)
        {
            throw new ArgumentException("Ability score ID cannot be default.", nameof(abilityScoreId));
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(abilityScoreAbbreviation);

        AbilityScoreId = abilityScoreId;
        AbilityScoreAbbreviation = abilityScoreAbbreviation;
    }

    public bool HasAssociatedAbilityScore => AbilityScoreId != default;

    internal static SavingThrow Create(RegistrationId associatedRegistrationId, string name, AbilityScoreId abilityScoreId, string abilityScoreAbbreviation)
        => new(associatedRegistrationId, name, abilityScoreId, abilityScoreAbbreviation);

    internal static SavingThrow CreateWithoutAbilityScore(RegistrationId associatedRegistrationId, string name)
        => new(associatedRegistrationId, name, default, default);
}
