namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>A content-defined candidate; it is not applied until selected.</summary>
public sealed record CharacterOptionResponse(string Id, string Name, string? Description);
