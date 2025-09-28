using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Components;
using Starlights.Modules.Characters.Domain.Progression.Eventing;

namespace Starlights.Modules.Characters.Domain.Progression;

public sealed class ProgressionComponent : CharacterComponentBase
{
    private ProgressionComponent(CharacterId parentCharacter)
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

        if (level == CharacterLevel)
        {
            return;
        }

        CharacterLevel = level;
        AddDomainEvent(new CharacterLevelChangedEvent { CharacterId = ParentCharacter, NewLevel = CharacterLevel });
    }

    /// <summary>
    /// Creates a new instance of the ProgressionComponent associated with the specified character.
    /// </summary>
    public static ProgressionComponent Create(CharacterId parentCharacter)
    {
        return new ProgressionComponent(parentCharacter);
    }
}
