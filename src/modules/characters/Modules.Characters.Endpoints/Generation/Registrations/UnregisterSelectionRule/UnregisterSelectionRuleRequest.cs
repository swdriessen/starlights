using FastEndpoints;

namespace Starlights.Modules.Characters.Endpoints.Generation.Registrations.UnregisterSelectionRule;

public class UnregisterSelectionRuleRequest
{
    [BindFrom("parentRegistration")]
    public required Guid ParentRegistration { get; set; }

    [BindFrom("elementId")]
    public required Guid ElementId { get; set; }
}