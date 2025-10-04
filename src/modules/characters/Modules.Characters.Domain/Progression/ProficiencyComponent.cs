using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Components;

namespace Starlights.Modules.Characters.Domain.Progression;

public sealed class ProficiencyComponent : CharacterComponentBase
{
    private ProficiencyComponent(CharacterId parentCharacter)
        : base(parentCharacter)
    {
    }

    /// <summary>
    /// Gets the proficiency bonus of the character.
    /// </summary>
    public int ProficiencyBonus { get; private set; } = 2;

    /// <summary>
    /// Sets the proficiency bonus for the character.
    /// </summary>
    public void SetProficiencyBonus(int bonus)
    {
        if (bonus < 2)
        {
            throw new Exception("Proficiency bonus cannot be less than 2.");
        }

        if (bonus == ProficiencyBonus)
        {
            return;
        }

        ProficiencyBonus = bonus;
        AddDomainEvent(new ProficiencyBonusChangedEvent(ProficiencyBonus) { CharacterId = ParentCharacter });
    }

    /// <summary>
    /// Creates a new instance of the ProficiencyComponent associated with the specified character.
    /// </summary>
    public static ProficiencyComponent Create(CharacterId parentCharacter)
    {
        return new ProficiencyComponent(parentCharacter);
    }
}
