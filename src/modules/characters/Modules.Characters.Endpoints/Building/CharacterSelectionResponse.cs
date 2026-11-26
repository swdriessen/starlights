namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>A stable occurrence-qualified Selection and its currently eligible options.</summary>
public sealed record CharacterSelectionResponse(string Id, string Name, int Count, IReadOnlyList<string> Answers, IReadOnlyList<CharacterOptionResponse> Options);
