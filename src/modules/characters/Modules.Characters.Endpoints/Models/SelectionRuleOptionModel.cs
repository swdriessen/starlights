namespace Starlights.Modules.Characters.Endpoints.Models;

/// <summary>
/// The DTO model for a selection rule option.
/// </summary>
public record SelectionRuleOptionModel
{
    public required Guid ElementId { get; init; }
    public required string Name { get; init; }
}
