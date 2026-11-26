namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>A recorded input and its resolved effective ability values.</summary>
public sealed record CharacterAbilityResponse(string Id, string Name, int? Input, int? Score, int? Modifier);
