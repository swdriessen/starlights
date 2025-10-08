using System.Diagnostics;
using System.Runtime.CompilerServices;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Platform.Domain;
using Starlights.Platform.SourceGenerators.Entities.Attributes;

namespace Starlights.Modules.Characters.Domain.Registrations;

/// <summary>
/// Represents a selection rule that has been applied to a registration via a selection rule component.
/// </summary>
[Entity]
[DebuggerDisplay("ElementType = {ElementType}, Name = {Name}, Id = {Id}, Parent = {ParentRegistrationId} RuleId = {AssociatedSelectionRuleId}")]
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
    /// Gets the registration id of the registration that was created as a result of this selection rule. This will be null if no selection has been made yet.
    /// </summary>
    public RegistrationId? SelectionRegistrationId { get; private set; }

    /// <summary>
    /// Gets the element id for the current selection made via this selection rule. This will be null if no selection has been made yet.
    /// </summary>
    public ElementId? SelectedOption { get; private set; }

    /// <summary>
    /// Updates the current selection for this selection rule.
    /// </summary>
    public void UpdateCurrentSelection(Registration selectionRegistration)
    {
        if (SelectionRegistrationId is not null)
        {
            throw new InvalidOperationException("Cannot update the current selection of a selection rule that already has a selection registration. Clear the current selection first.");
        }

        if (SelectedOption is not null)
        {
            throw new InvalidOperationException("Cannot update the current selection of a selection rule that already has a selection. Clear the current selection first.");
        }

        SelectionRegistrationId = selectionRegistration.Id;
        SelectedOption = selectionRegistration.AssociatedElementId;
    }

    public void ClearCurrentSelection()
    {
        SelectionRegistrationId = null;
        SelectedOption = null;
    }

    public bool HasCurrentSelection()
    {
        return SelectionRegistrationId is not null && SelectedOption is not null;
    }

    /// <summary>
    /// Factory for creating a new applied registration selection rule.
    /// </summary>
    internal static RegistrationSelectionRule Create(RegistrationId parentRegistrationId, ElementComponentId associatedSelectionRuleId, string elementType, string name)
    {
        RegistrationSelectionRule registrationSelectionRule = new(parentRegistrationId, associatedSelectionRuleId, elementType, name);

        // domain event?

        return registrationSelectionRule;
    }
}
