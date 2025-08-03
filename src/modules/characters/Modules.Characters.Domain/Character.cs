using Starlights.Platform.Domain;

namespace Starlights.Modules.Characters.Domain;

/// <summary>
/// Represents a character in the system.
/// </summary>
public sealed class Character : AggregateRoot<CharacterId>
{
    private Character(string name)
        : base(CharacterId.New())
    {
        Name = name;
    }

    /// <summary>
    /// Gets the name of the character.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Creates a new instance of the <see cref="Character"/> class with the specified name.
    /// </summary>
    public static Character Create(string name)
    {
        return new Character(name);
    }
}
