using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Building;
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
    public string Name { get; private set; } = string.Empty;

    /// <summary>Gets the account owning this Character.</summary>
    public Guid OwnerId { get; private set; }

    /// <summary>Gets the canonical Ruleset, or null for a retained legacy Character.</summary>
    public string? RulesetId { get; private set; }

    /// <summary>Gets validated initialization state, when loaded for a Ruleset-based build.</summary>
    public CharacterBuild? Build { get; private set; }

    /// <summary>Creates a valid incomplete Character without starting legacy background processing.</summary>
    public static Character Create(Guid ownerId, string? name, CharacterBuild build)
    {
        if (ownerId == Guid.Empty)
            throw new ArgumentException("An authenticated owner is required.", nameof(ownerId));
        if (name?.Length > 100)
            throw new ArgumentException("Character name cannot exceed 100 characters.", nameof(name));
        return new Character(string.IsNullOrWhiteSpace(name) ? "The Nameless One" : name)
        {
            OwnerId = ownerId,
            RulesetId = build.RulesetId,
            Build = build
        };
    }

    /// <summary>Reconstitutes authoritative state without initialization or new domain events.</summary>
    public static Character Reconstitute(CharacterId id, Guid ownerId, string name, CharacterBuild build)
    {
        var character = Create(ownerId, name, build);
        return new Character(id, character.Name)
        {
            OwnerId = ownerId,
            RulesetId = build.RulesetId,
            Build = build
        };
    }

    private Character(CharacterId id, string name) : base(id) => Name = name;

    /// <summary>Records a validated Choice through the selected Ruleset's policy.</summary>
    public void RecordChoice(ICharacterBuildRules rules, string selectionId, IReadOnlyList<string> answers)
        => ReplaceBuild(rules, () => rules.RecordChoice(Build!.Composition, selectionId, answers));

    /// <summary>Assigns validated ability inputs without exposing mutable Ruleset parts.</summary>
    public void AssignAbilityScores(ICharacterBuildRules rules, IReadOnlyDictionary<string, int> inputs)
        => ReplaceBuild(rules, () => rules.AssignAbilities(Build!.Composition, inputs));

    /// <summary>Claims a starting package once, separately from the selected option.</summary>
    public void ClaimEquipment(ICharacterBuildRules rules, string selectionId)
        => ReplaceBuild(rules, () => rules.ClaimEquipment(Build!.Composition, selectionId));

    /// <summary>Allocates owned armor and a weapon using the Ruleset's equipment rules.</summary>
    public void Equip(ICharacterBuildRules rules, string? armorId, string? weaponId)
        => ReplaceBuild(rules, () => rules.Equip(Build!.Composition, armorId, weaponId));

    /// <summary>Names an initialized Character without affecting its identity.</summary>
    public void Rename(string name)
    {
        if (Build is null || string.IsNullOrWhiteSpace(name) || name.Length > 100)
            throw new ArgumentException("A Character name must contain 1 to 100 characters.", nameof(name));
        Name = name;
    }

    private void ReplaceBuild(ICharacterBuildRules rules, Func<ICharacterComposition> operation)
    {
        var build = Build ?? throw new InvalidOperationException("Legacy Characters do not support build actions.");
        if (rules.RulesetId != build.RulesetId || rules.Content != build.Content)
            throw new InvalidOperationException("The build policy does not match the Character's pinned content.");
        var composition = operation();
        Build = new CharacterBuild(rules, build.Content, composition);
    }

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
        if (RulesetId is not null)
            throw new InvalidOperationException("Legacy components cannot be attached to a Ruleset-based Character.");
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
