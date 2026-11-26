namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>A separately retained concise content reminder.</summary>
public sealed record CharacterSheetEntryResponse(string Name, string? Description, string? Usage, string Id = "");
