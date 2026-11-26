namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>An independently identified possession and its supported equipment roles.</summary>
public sealed record CharacterOwnedItemResponse(string Id, string Name, int Quantity, bool CanWear, bool CanWield);
