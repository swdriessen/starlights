using Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;

namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>A content-defined supported creation option, not a promise of implemented game mechanics.</summary>
public sealed record RulesetOptionResponse(string RulesetId, string Name, string PackName, CharacterContentResponse Content);
