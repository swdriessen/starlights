using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes.Eventing;
using Starlights.Modules.Characters.Domain.Components;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Domain.Classes;

public class ClassComponent : CharacterComponentBase
{
    private readonly List<CharacterClass> _classes = [];

    public ClassComponent(CharacterId parentCharacter)
        : base(parentCharacter)
    {

    }

    /// <summary>
    /// Gets the collection of classes associated with the character.
    /// </summary>
    public IReadOnlyCollection<CharacterClass> Classes => _classes.AsReadOnly();

    /// <summary>
    /// Gets the combined level of all classes associated with the character.
    /// </summary>
    public int GetCombinedLevel()
    {
        return _classes.Sum(c => c.Level);
    }

    /// <summary>
    /// Adds a new class to the character's collection of classes.
    /// </summary>
    public CharacterClass AddClass(RegistrationId registration, string name, IEventRecorder eventRecorder)
    {
        var newClass = CharacterClass.Create(registration, name);

        if (_classes.Count == 0)
        {
            newClass.SetPrimary(true);
        }

        _classes.Add(newClass);

        eventRecorder.AddDomainEvent(new CharacterClassCreatedEvent() { CharacterId = ParentCharacter, ClassId = newClass.Id });

        return newClass;
    }
}
