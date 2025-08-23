using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Platform.Domain;
using Starlights.Platform.SourceGenerators.Entities.Attributes;

namespace Starlights.Modules.Characters.Domain.Registrations;

/// <summary>
/// Represents a selection rule that has been applied to a registration via a selection rule component.
/// </summary>
[Entity]
[DebuggerDisplay("Id = {Id}, Parent = {ParentRegistrationId} RuleId = {AssociatedSelectionRuleId}, ElementType = {ElementType}, Name = {Name}")]
public sealed class RegistrationSelectionRule : EntityBase<RegistrationSelectionRuleId>
{
    private RegistrationSelectionRule(RegistrationId parentRegistrationId, ElementComponentId associatedSelectionRuleId, string elementType, string name)
        : base(RegistrationSelectionRuleId.New())
    {
        ParentRegistrationId = parentRegistrationId;
        AssociatedSelectionRuleId = associatedSelectionRuleId;
        ElementType = elementType.Trim();
        Name = name.Trim();
    }

    /// <summary>
    /// Gets the parent registration associated with this selection rule.
    /// </summary>
    public RegistrationId ParentRegistrationId { get; }

    /// <summary>
    /// Gets the ID of the selection rule component that produced this applied rule.
    /// </summary>
    public ElementComponentId AssociatedSelectionRuleId { get; }

    /// <summary>
    /// Gets the type of element this selection rule applies to.
    /// </summary>
    public string ElementType { get; }

    /// <summary>
    /// Gets the name of the selection rule.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Factory for creating a new applied registration selection rule.
    /// </summary>
    internal static RegistrationSelectionRule Create(RegistrationId parentRegistrationId, ElementComponentId associatedSelectionRuleId, string elementType, string name)
        => new(parentRegistrationId, associatedSelectionRuleId, elementType, name);
}
