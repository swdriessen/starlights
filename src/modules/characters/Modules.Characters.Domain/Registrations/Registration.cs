using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Platform.Domain;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Domain.Registrations;

/// <summary>
/// Represents a registration for an element in the system.
/// </summary>
[DebuggerDisplay("Id = {Id}, AssociatedElementName = {AssociatedElementName}, Parent = {ParentRegistrationId}")]
public sealed class Registration : AggregateRoot<RegistrationId>
{
    private readonly List<RegistrationIncludeRule> _includeRules = [];

    private Registration(CharacterId characterId, ElementId associatedElementId, string associatedElementName, string associatedElementType)
        : base(RegistrationId.New())
    {
        CharacterId = characterId;
        AssociatedElementId = associatedElementId;
        AssociatedElementName = associatedElementName;
        AssociatedElementType = associatedElementType;
    }

    /// <summary>
    /// Gets the collection of include rules associated with this registration.
    /// </summary>
    public IReadOnlyCollection<RegistrationIncludeRule> IncludeRules => _includeRules.AsReadOnly();

    /// <summary>
    /// Gets the ID of the character associated with this registration.
    /// </summary>
    public CharacterId CharacterId { get; }

    /// <summary>
    /// Gets the ID of the parent registration, if this registration is part of a hierarchy (e.g. the root element or manually added character options have no direct parent).
    /// </summary>
    public RegistrationId? ParentRegistrationId { get; private set; }

    /// <summary>
    /// Gets the ID of the element associated with this registration.
    /// </summary>
    public ElementId AssociatedElementId { get; }

    /// <summary>
    /// Gets the name of the element associated with this registration. This is used for display purposes and may not be unique.
    /// </summary>
    public string AssociatedElementName { get; }

    /// <summary>
    /// Gets the type of the element associated with this registration. This is used to determine how to handle the registration.
    /// </summary>
    public string AssociatedElementType { get; }

    /// <summary>
    /// Updates the parent registration ID for this registration.
    /// </summary>
    public void UpdateParentRegistration(Registration parentRegistration)
    {
        ParentRegistrationId = parentRegistration.Id;
    }

    /// <summary>
    /// Indicates whether this registration has been processed after it has been added.
    /// </summary>
    public bool IsProcessed { get; private set; }

    /// <summary>
    /// Marks this registration as processed, indicating that it has been initially processed after registration.
    /// </summary>
    public void Processed()
    {
        IsProcessed = true;
        AddDomainEvent(new RegistrationProcessed { CharacterId = CharacterId, RegistrationId = Id });
    }

    /// <summary>
    /// Gets a value indicating whether this registration has a specific include rule by its associated rule ID.
    /// </summary>
    public bool HasAssociatedRule(Guid associatedRuleId)
    {
        var hasRule = _includeRules.Any(x => x.AssociatedIncludeRuleId.Value == associatedRuleId);

        // TODO: add checks once we have multiple rule types e.g. selection, statistic, etc.

        return hasRule;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Registration"/> class with the specified character ID, element ID, and element name.
    /// </summary>
    public static Registration Create(CharacterId characterId, ElementId associatedElementId, string associatedElementName, string associatedElementType, Func<Registration, IEnumerable<IDomainEvent>>? eventInjector = null)
    {
        var newRegistration = new Registration(characterId, associatedElementId, associatedElementName, associatedElementType);

        newRegistration.AddDomainEvent(new RegistrationCreated
        {
            CharacterId = newRegistration.CharacterId,
            RegistrationId = newRegistration.Id,
            AssociatedElementName = associatedElementName,
            AssociatedElementType = associatedElementType
        });

        if (eventInjector is not null)
        {
            foreach (var additionalEvent in eventInjector(newRegistration))
            {
                newRegistration.AddDomainEvent(additionalEvent);
            }
        }

        return newRegistration;
    }

    /// <summary>
    /// Creates a new include rule for this registration.
    /// </summary>
    public RegistrationIncludeRule CreateIncludeRule(ElementComponentId associatedIncludeRuleId, ElementId includedElementId, string includedElementName)
    {
        var newIncludeRule = RegistrationIncludeRule.Create(Id, associatedIncludeRuleId, includedElementId, includedElementName);
        _includeRules.Add(newIncludeRule);
        return newIncludeRule;
    }
}
