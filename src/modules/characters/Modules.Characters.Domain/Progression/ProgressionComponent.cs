using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Components;

namespace Starlights.Modules.Characters.Domain.Progression;

public class ProgressionComponent : CharacterComponentBase
{
    public ProgressionComponent(CharacterId parentCharacter)
        : base(parentCharacter)
    {

    }

    /// <summary>
    /// Gets the current level of the character.
    /// </summary>
    public int CharacterLevel { get; private set; }

    /// <summary>
    /// Sets the character's level to the specified value.
    /// </summary>
    public void SetCharacterLevel(int level)
    {
        if (level < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(level), "Character level cannot be negative.");
        }

        CharacterLevel = level;
    }
}
