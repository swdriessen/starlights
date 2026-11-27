namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>Saved generation method, persistent dice, and point-buy accounting.</summary>
public sealed record CharacterAbilityGenerationResponse(
    string Method, IReadOnlyList<CharacterAbilityRollResponse> Rolls, int? PointsSpent, int? PointsRemaining);
