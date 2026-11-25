namespace Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;

/// <summary>The exact content pinned to a saved Character.</summary>
public sealed record CharacterContentResponse(string PackId, string PackVersion, string CharacterElementId);
