using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Platform.Domain;
using Starlights.Platform.SourceGenerators.Entities.Attributes;

namespace Starlights.Modules.Characters.Domain.Registrations;

/// <summary>
/// Represents a registration for an element in the system.
/// </summary>
[Entity]
[DebuggerDisplay("Element = {AssociatedElementName} ({AssociatedElementType}), Includes = {IncludeRules.Count}, Selections = {SelectionRules.Count}, Statistics = {StatisticRules.Count}")]
public sealed class Registration : AggregateRoot<RegistrationId>
{
    private readonly List<RegistrationIncludeRule> _includeRules = [];
    private readonly List<RegistrationStatisticRule> _statisticRules = [];
    private readonly List<RegistrationSelectionRule> _selectionRules = [];

    private Registration(CharacterId characterId, ElementId associatedElementId, string associatedElementName, string associatedElementType)
        : base(RegistrationId.New())
    {
        CharacterId = characterId;
        AssociatedElementId = associatedElementId;
        AssociatedElementName = associatedElementName;
        AssociatedElementType = associatedElementType;
    }

    /// <summary>
    /// Gets the ID of the character associated with this registration.
    /// </summary>
    public CharacterId CharacterId { get; }

    /// <summary>
    /// Gets the collection of include rules associated with this registration.
    /// </summary>
    public IReadOnlyCollection<RegistrationIncludeRule> IncludeRules => _includeRules.AsReadOnly();

    /// <summary>
    /// Gets the collection of statistic rules associated with this registration.
    /// </summary>
    public IReadOnlyCollection<RegistrationStatisticRule> StatisticRules => _statisticRules.AsReadOnly();

    /// <summary>
    /// Gets the collection of selection rules associated with this registration.
    /// </summary>
    public IReadOnlyCollection<RegistrationSelectionRule> SelectionRules => _selectionRules.AsReadOnly();

    /// <summary>
    /// Gets or sets the progression origin registration ID used for level-gated rule evaluation.
    /// When set, level requirements are evaluated against the level of the origin (e.g., a class) rather than the character.
    /// </summary>
    public RegistrationId? ProgressionOriginRegistrationId { get; private set; }

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
    /// Gets a value indicating whether any include rules are defined.
    /// </summary>
    public bool HasIncludeRules()
    {
        return _includeRules.Count > 0;
    }

    /// <summary>
    /// Gets a value indicating whether any statistic rules are defined.
    /// </summary>
    public bool HasStatisticRules()
    {
        return _statisticRules.Count > 0;
    }

    /// <summary>
    /// Gets a value indicating whether any selection rules are defined.
    /// </summary>
    public bool HasSelectionRules()
    {
        return _selectionRules.Count > 0;
    }

    /// <summary>
    /// Sets the parent registration ID for this registration.
    /// </summary>
    public void SetParentRegistration(Registration parentRegistration)
    {
        ParentRegistrationId = parentRegistration.Id;
    }

    /// <summary>
    /// Updates the progression origin registration for this registration.
    /// </summary>
    public void SetProgressionOrigin(RegistrationId originRegistrationId)
    {
        ProgressionOriginRegistrationId = originRegistrationId;
    }

    /// <summary>
    /// Updates the progression origin registration for this registration.
    /// </summary>
    public void SetProgressionOrigin(Registration originRegistration)
    {
        // determine progression origin based on parent
        var capableTypes = new[] { "class" }; // this should come from system based config or similar

        if (originRegistration.ProgressionOriginRegistrationId is not null)
        {
            SetProgressionOrigin(originRegistration.ProgressionOriginRegistrationId.Value);
        }
        else if (capableTypes.Contains(originRegistration.AssociatedElementType, StringComparer.OrdinalIgnoreCase))
        {
            SetProgressionOrigin(originRegistration.Id);
        }
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
        AddDomainEvent(new RegistrationProcessedEvent
        {
            CharacterId = CharacterId,
            RegistrationId = Id,
            ElementName = AssociatedElementName,
            ElementType = AssociatedElementType
        });
    }

    /// <summary>
    /// Marks the registration as deleted by raising a domain event indicating its deletion.
    /// </summary>
    public void MarkDeleted()
    {
        AddDomainEvent(new RegistrationDeletedEvent
        {
            CharacterId = CharacterId,
            RegistrationId = Id,
            AssociatedElementName = AssociatedElementName,
            AssociatedElementType = AssociatedElementType
        });
    }

    /// <summary>
    /// Gets a value indicating whether this registration has a specific include/statistic/selection rule by its associated rule ID.
    /// </summary>
    public bool HasAssociatedRule(Guid associatedRuleId)
    {
        return _includeRules.Any(r => r.AssociatedIncludeRuleId.Value == associatedRuleId)
               || _statisticRules.Any(r => r.AssociatedStatisticRuleId.Value == associatedRuleId)
               || _selectionRules.Any(r => r.AssociatedSelectionRuleId.Value == associatedRuleId);
    }

    /// <summary>
    /// Creates a new include rule for this registration.
    /// </summary>
    public RegistrationIncludeRule CreateIncludeRule(ElementComponentId associatedIncludeRuleId, ElementId includedElementId, string includedElementName)
    {
        var newIncludeRule = RegistrationIncludeRule.Create(Id, associatedIncludeRuleId, includedElementId, includedElementName);
        _includeRules.Add(newIncludeRule);

        AddDomainEvent(new RegistrationIncludeRuleCreatedEvent
        {
            CharacterId = CharacterId,
            RegistrationId = Id,
            RegistrationIncludeRuleId = newIncludeRule.Id,
            ElementId = includedElementId,
            Name = includedElementName
        });

        return newIncludeRule;
    }

    /// <summary>
    /// Creates a new statistic rule for this registration.
    /// </summary>
    public RegistrationStatisticRule CreateStatisticRule(ElementComponentId associatedStatisticRuleId, string name, string value)
    {
        var newStatisticRule = RegistrationStatisticRule.Create(Id, associatedStatisticRuleId, name, value);
        _statisticRules.Add(newStatisticRule);

        AddDomainEvent(new RegistrationStatisticRuleCreatedEvent
        {
            CharacterId = CharacterId,
            RegistrationId = Id,
            RegistrationStatisticRuleId = newStatisticRule.Id,
            Name = name,
            Value = value
        });

        return newStatisticRule;
    }

    /// <summary>
    /// Creates a new selection rule for this registration.
    /// </summary>
    public RegistrationSelectionRule CreateSelectionRule(ElementComponentId associatedSelectionRuleId, string elementType, string name)
    {
        var newSelectionRule = RegistrationSelectionRule.Create(Id, associatedSelectionRuleId, elementType, name);
        _selectionRules.Add(newSelectionRule);

        AddDomainEvent(new RegistrationSelectionRuleCreatedEvent
        {
            CharacterId = CharacterId,
            RegistrationId = Id,
            RegistrationSelectionRuleId = newSelectionRule.Id,
            ElementType = elementType,
            Name = name
        });

        return newSelectionRule;
    }







    public void RemoveIncludeRule(RegistrationIncludeRule includeRule)
    {
        if (_includeRules.Remove(includeRule))
        {
            AddDomainEvent(new RegistrationIncludeRuleDeletedEvent
            {
                CharacterId = CharacterId,
                RegistrationId = Id,
                RegistrationIncludeRuleId = includeRule.Id,
                ElementId = includeRule.IncludedElementId,
                Name = includeRule.IncludedElementName
            });
        }
    }

    public void RemoveStatisticRule(RegistrationStatisticRule statisticRule)
    {
        if (_statisticRules.Remove(statisticRule))
        {
            AddDomainEvent(new RegistrationStatisticRuleDeletedEvent
            {
                CharacterId = CharacterId,
                RegistrationId = Id,
                RegistrationStatisticRuleId = statisticRule.Id,
                Name = statisticRule.Name,
                Value = statisticRule.Value
            });
        }
    }

    public void RemoveSelectionRule(RegistrationSelectionRule selectionRule)
    {
        if (_selectionRules.Remove(selectionRule))
        {
            AddDomainEvent(new RegistrationSelectionRuleDeletedEvent
            {
                CharacterId = CharacterId,
                RegistrationId = Id,
                RegistrationSelectionRuleId = selectionRule.Id,
                ElementType = selectionRule.ElementType,
                Name = selectionRule.Name
            });
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Registration"/> class with the specified character ID, element ID, and element name.
    /// </summary>
    public static Registration Create(CharacterId characterId, ElementId associatedElementId, string associatedElementName, string associatedElementType)
    {
        var newRegistration = new Registration(characterId, associatedElementId, associatedElementName, associatedElementType);

        newRegistration.AddDomainEvent(new RegistrationCreatedEvent
        {
            CharacterId = newRegistration.CharacterId,
            RegistrationId = newRegistration.Id,
            AssociatedElementName = associatedElementName,
            AssociatedElementType = associatedElementType
        });

        return newRegistration;
    }


    /// <summary>
    /// When reprocessing registrations, this field tracks the originating rule that caused this registration to be created.
    /// In case the originating rule is deleted, this allows us to identify and remove registrations that were created as a result of that rule.
    /// </summary>
    /// <remarks>
    /// This can currently be an include rule or a selection rule.
    /// </remarks>
    public Guid? OriginatingRule { get; private set; }

    /// <summary>
    /// Sets the originating rule identifier for this instance.
    /// </summary>
    /// <param name="originatingRuleId">The unique identifier of the originating rule to associate with this instance.</param>
    public void SetOriginatingRule(Guid originatingRuleId)
    {
        if (originatingRuleId == Guid.Empty)
        {
            throw new ArgumentException("The originating rule ID cannot be an empty GUID.", nameof(originatingRuleId));
        }

        if (OriginatingRule is not null)
        {
            throw new InvalidOperationException("The originating rule has already been set and cannot be changed.");
        }

        OriginatingRule = originatingRuleId;
    }
}


