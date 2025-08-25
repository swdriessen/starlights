namespace Starlights.Modules.Characters.Endpoints.Models;

/// <summary>
/// The DTO model for a selection rule.
/// </summary>
public record SelectionRuleDataModel
{
    public Guid RegistrationId { get; set; }
    public Guid RegistrationSelectionRuleId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public Guid? ActiveRegistration { get; set; }
}
