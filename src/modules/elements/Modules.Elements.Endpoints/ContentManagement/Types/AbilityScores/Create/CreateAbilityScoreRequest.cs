namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores.Create;

public sealed record CreateAbilityScoreRequest(
    string Name,
    string Abbreviation,
    string? Description);
