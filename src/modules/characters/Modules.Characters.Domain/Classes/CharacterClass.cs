using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Domain;
using Starlights.Platform.SourceGenerators.Entities.Attributes;

namespace Starlights.Modules.Characters.Domain.Classes;

[Entity]
[DebuggerDisplay("Class: {Name}, Level: {Level}, IsPrimary: {IsPrimary}")]
public class CharacterClass : EntityBase<CharacterClassId>
{
    public CharacterClass(RegistrationId registration, string name)
        : base(CharacterClassId.New())
    {
        Registration = registration;
        Name = name;
    }

    /// <summary>
    /// Gets the id of the registration element associated with the class.
    /// </summary>
    public RegistrationId Registration { get; }

    /// <summary>
    /// Gets the name of the class.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a value indicating whether the class is the primary class of the character.
    /// </summary>
    public bool IsPrimary { get; private set; }

    /// <summary>
    /// Gets the level of the class.
    /// </summary>
    public int Level { get; private set; } = 1;

    /// <summary>
    /// Sets the class as the primary class of the character.
    /// </summary>
    public void SetPrimary(bool isPrimary = true)
    {
        IsPrimary = isPrimary;
    }

    /// <summary>
    /// Updates the level of the class to the specified value.
    /// </summary>
    public void UpdateLevel(int value)
    {
        Level = value;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="CharacterClass"/> entity.
    /// </summary>
    public static CharacterClass Create(RegistrationId registration, string name) => new(registration, name);
}
