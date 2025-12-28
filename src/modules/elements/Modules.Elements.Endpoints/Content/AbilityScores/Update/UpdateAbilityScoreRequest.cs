namespace Starlights.Modules.Elements.Endpoints.Content.AbilityScores.Update;

public sealed record UpdateAbilityScoreRequest(
    Guid Id,
    string Name,
    string Abbreviation,
    string? Description);
