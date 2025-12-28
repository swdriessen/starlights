namespace Starlights.Modules.Elements.Endpoints.Content.AbilityScores.Create;

public sealed record CreateAbilityScoreRequest(
    string Name,
    string Abbreviation,
    string? Description);
