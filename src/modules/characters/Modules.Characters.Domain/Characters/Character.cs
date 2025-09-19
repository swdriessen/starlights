using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Characters.Eventing;
using Starlights.Modules.Characters.Domain.Components;
using Starlights.Platform.Domain;
using Starlights.Platform.Eventing;
using Starlights.Platform.SourceGenerators.Entities.Attributes;

namespace Starlights.Modules.Characters.Domain.Characters;

/// <summary>
/// Represents a character in the system.
/// </summary>
[Entity]
[DebuggerDisplay("Id = {Id}, Name = {Name}")]
public sealed class Character : AggregateRoot<CharacterId>
{
    private readonly List<CharacterComponentBase> _components = [];

    private Character(string name)
        : base(CharacterId.New())
    {
        Name = name;
    }

    /// <summary>
    /// Gets the collection of components associated with the character.
    /// </summary>
    public IReadOnlyCollection<CharacterComponentBase> Components => _components.AsReadOnly();

    /// <summary>
    /// Gets the name of the character.
    /// </summary>
    public string Name { get; } = string.Empty;

    /// <summary>
    /// Creates a new instance of the <see cref="Character"/> class with the specified name.
    /// </summary>
    public static Character Create(string name)
    {
        var newCharacter = new Character(name);
        newCharacter.AddDomainEvent(new CharacterCreatedEvent() { CharacterId = newCharacter.Id });
        return newCharacter;
    }

    #region Components

    /// <summary>
    /// Adds a component to the character.
    /// </summary>
    public T AddComponent<T>(T component) where T : CharacterComponentBase
    {
        if (component.ParentCharacter != Id)
        {
            throw new InvalidOperationException("Component's CharacterId does not match this Character's Id.");
        }

        _components.Add(component);
        return component;
    }

    /// <summary>
    /// Gets a single component of the specified type.
    /// </summary>
    public T GetRequiredComponent<T>() where T : CharacterComponentBase
    {
        return _components.OfType<T>().Single();
    }

    /// <summary>
    /// Gets all components of the specified type.
    /// </summary>
    public IEnumerable<T> GetComponents<T>() where T : CharacterComponentBase => _components.OfType<T>();

    /// <summary>
    /// Executes an action on a component of the specified type and adds any resulting domain events.
    /// </summary>
    public T UpdateComponent<T>(Action<T, IEventRecorder> component) where T : CharacterComponentBase
    {
        var existingComponent = GetRequiredComponent<T>();
        component(existingComponent, new EventRecorder(this));
        return existingComponent;
    }

    /// <summary>
    /// Executes an action on a component of the specified type and adds any resulting domain events.
    /// </summary>
    public void UpdateComponents<T1, T2>(Action<T1, T2, IEventRecorder> component)
        where T1 : CharacterComponentBase
        where T2 : CharacterComponentBase
    {
        var existingComponent1 = GetRequiredComponent<T1>();
        var existingComponent2 = GetRequiredComponent<T2>();
        component(existingComponent1, existingComponent2, new EventRecorder(this));
    }

    private sealed class EventRecorder : IEventRecorder
    {
        private readonly Character _character;

        public EventRecorder(Character character)
        {
            _character = character;
        }

        public void AddDomainEvent(IDomainEvent domainEvent) => _character.AddDomainEvent(domainEvent);
    }

    #endregion
}
