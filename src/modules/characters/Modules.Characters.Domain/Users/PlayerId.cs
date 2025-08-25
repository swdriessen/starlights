using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Components;

namespace Starlights.Modules.Characters.Domain.Users;

/// <summary>
/// Represents a unique identifier for players in the context of the Characters module.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct PlayerId(Guid Value)
{
    /// <summary>
    /// Implicitly converts PlayerId to Guid.
    /// </summary>
    public static implicit operator Guid(PlayerId id) => id.Value;
}

public class PlayerComponent : CharacterComponentBase
{
    public PlayerComponent(CharacterId parentCharacter)
        : base(parentCharacter)
    {

    }

    /// <summary>
    /// Gets the name of the player associated with the character.
    /// </summary>
    public string PlayerName { get; private set; } = string.Empty;

    public void UpdatePlayerName(string newName)
    {
        PlayerName = newName;
    }
}
