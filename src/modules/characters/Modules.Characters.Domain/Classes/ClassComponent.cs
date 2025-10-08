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
    /// Updates the level of the specified class to the new level.
    /// </summary>
    public void LevelUpClass(CharacterClassId classId, int newLevel)
    {
        var characterClass = _classes.SingleOrDefault(c => c.Id == classId)
            ?? throw new InvalidOperationException($"Character does not have a class with ID {classId}");

        if (newLevel <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newLevel), "New level must be a positive integer.");
        }

        // TODO: return bool if updated, nothing when level is same as current
        characterClass.UpdateLevel(newLevel);
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
    /// Removes the class associated with the specified registration ID from the character.
    /// </summary>
    /// <remarks>If the removed class is the character's primary class and other classes remain, a new primary
    /// class may need to be set. This method also raises a domain event to signal that a class has been
    /// removed.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if no class is associated with the specified registration ID.</exception>
    public void RemoveClass(RegistrationId existinRegistration)
    {
        var existingClass = _classes.SingleOrDefault(c => c.Registration == existinRegistration) 
            ?? throw new InvalidOperationException($"Character does not have a class associated with registration ID {existinRegistration}");

        // TODO: multiclassing, if the class being removed is the primary class, we need to set a new primary class if there are any classes left

        _classes.Remove(existingClass);

        AddDomainEvent(new CharacterClassRemovedEvent() { CharacterId = ParentCharacter, ClassId = existingClass.Id });
    }

    /// <summary>
    /// Creates a new instance of the ClassComponent associated with the specified character.
    /// </summary>
    public static ClassComponent Create(CharacterId parentCharacter)
    {
        return new ClassComponent(parentCharacter);
    }
}
