using FastEndpoints;

namespace Starlights.Modules.Characters.Endpoints.Generation.Registrations.RegisterSelectionRule;

public class RegisterSelectionRuleRequest
{
    [BindFrom("parentRegistration")]
    public required Guid ParentRegistration { get; set; }

    [BindFrom("elementId")]
    public required Guid ElementId { get; set; }
}