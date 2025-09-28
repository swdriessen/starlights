using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes.Eventing;
using Starlights.Modules.Characters.Domain.Components;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Domain.Classes;

public sealed class ClassComponent : CharacterComponentBase
{
    private readonly List<CharacterClass> _classes = [];

    private ClassComponent(CharacterId parentCharacter)
        : base(parentCharacter)
    {

    }

    /// <summary>
    /// Gets the collection of classes associated with the character.
    /// </summary>
    public IReadOnlyCollection<CharacterClass> Classes => _classes.AsReadOnly();

    /// <summary>
    /// Gets a value indicating whether the character has multiple classes.
    /// </summary>
    public bool IsMulticlass => _classes.Count > 1;

    /// <summary>
    /// Calculates the combined level of all classes associated with the character.
    /// </summary>
    public int CalculateCharacterLevel()
    {
        return _classes.Sum(c => c.Level);
    }

    /// <summary>
    /// Adds a new class to the character's collection of classes.
    /// </summary>
    public CharacterClass CreateClass(RegistrationId registration, string name)
    {
        var newClass = CharacterClass.Create(registration, name);

        if (_classes.Count == 0)
        {
            newClass.SetPrimary(true);
        }

        _classes.Add(newClass);

        AddDomainEvent(new CharacterClassCreatedEvent() { CharacterId = ParentCharacter, ClassId = newClass.Id });

        return newClass;
    }

    /// <summary>
    /// Creates a new instance of the ClassComponent associated with the specified character.
    /// </summary>
    public static ClassComponent Create(CharacterId parentCharacter)
    {
        return new ClassComponent(parentCharacter);
    }
}
