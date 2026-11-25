using System.Text.Json;
using Starlights.Modules.Characters.Domain.Building;

namespace Starlights.Modules.Characters.Building;

/// <summary>A validated content-defined creation option; its declarations are retained, not evaluated.</summary>
public sealed record CharacterCreationOption(
    string RulesetId, string Name, string PackName, CharacterContentBinding Content, JsonElement Definition);
