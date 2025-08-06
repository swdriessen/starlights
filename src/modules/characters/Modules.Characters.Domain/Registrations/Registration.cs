using System.Diagnostics;
using Starlights.Platform.Domain;

namespace Starlights.Modules.Characters.Domain.Registrations;

/// <summary>
/// Represents a registration for an element in the system.
/// </summary>
[DebuggerDisplay("Id = {Id}, AssociatedElementName = {AssociatedElementName}, Parent = {ParentRegistrationId}")]
public sealed class Registration : AggregateRoot<RegistrationId>
{
    private readonly List<RegistrationIncludeRule> _includeRules = [];

    private Registration(CharacterId characterId, ElementId associatedElementId, string associatedElementName)
        : base(RegistrationId.New())
    {
        CharacterId = characterId;
        AssociatedElementId = associatedElementId;
        AssociatedElementName = associatedElementName;
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
    /// Creates a new instance of the <see cref="Registration"/> class with the specified character ID, element ID, and element name.
    /// </summary>
    public static Registration Create(CharacterId characterId, ElementId associatedElementId, string associatedElementName)
    {
        return new Registration(characterId, associatedElementId, associatedElementName);
    }

    /// <summary>
    /// Updates the parent registration ID for this registration.
    /// </summary>
    public void UpdateParentRegistration(Registration parentRegistration)
    {
        ParentRegistrationId = parentRegistration.Id;
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
