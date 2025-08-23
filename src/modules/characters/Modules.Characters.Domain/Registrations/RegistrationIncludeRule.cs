using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Platform.Domain;
using Starlights.Platform.SourceGenerators.Entities.Attributes;

namespace Starlights.Modules.Characters.Domain.Registrations;

/// <summary>
/// Represents a rule for an element in the system.
/// </summary>
[Entity]
[DebuggerDisplay("Id = {Id}, Parent = {ParentRegistrationId} RuleId = {AssociatedIncludeRuleId}, ElementName = {IncludedElementName}, ElementId = {IncludedElementId}")]
public sealed class RegistrationIncludeRule : EntityBase<RegistrationIncludeRuleId>
{
    private RegistrationIncludeRule(RegistrationId parentRegistrationId, ElementComponentId associatedIncludeRuleId, ElementId includedElementId, string includedElementName)
        : base(RegistrationIncludeRuleId.New())
    {
        ParentRegistrationId = parentRegistrationId;
        AssociatedIncludeRuleId = associatedIncludeRuleId;
        IncludedElementId = includedElementId;
        IncludedElementName = includedElementName;
    }

    /// <summary>
    /// Gets the parent registration associated with this rule.
    /// </summary>
    public RegistrationId ParentRegistrationId { get; }

    /// <summary>
    /// Gets the ID of the associated element component associated with this rule.
    /// </summary>
    public ElementComponentId AssociatedIncludeRuleId { get; }

    /// <summary>
    /// Gets the ID of the element that is included with this rule.
    /// </summary>
    public ElementId IncludedElementId { get; }

    /// <summary>
    /// Gets the name of the element associated with this rule. This is used for display purposes and may not be unique.
    /// </summary>
    public string IncludedElementName { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="RegistrationIncludeRule"/> class with the specified element ID and element name.
    /// </summary>
    /// <remarks>
    /// This method is typically used when creating a new include rule for a registration, such as when processing an element's include rules.
    /// </remarks>
    internal static RegistrationIncludeRule Create(RegistrationId parentRegistrationId, ElementComponentId associatedIncludeRuleId, ElementId includedElementId, string includedElementName) => new(parentRegistrationId, associatedIncludeRuleId, includedElementId, includedElementName);
}
