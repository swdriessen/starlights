namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores.Update;

public sealed record UpdateAbilityScoreRequest(
    Guid Id,
    string Name,
    string Abbreviation,
    string? Description);
