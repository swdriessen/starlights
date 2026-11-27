namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>Original application dice and the total after dropping one lowest die.</summary>
public sealed record CharacterAbilityRollResponse(IReadOnlyList<int> Dice, int Total);
