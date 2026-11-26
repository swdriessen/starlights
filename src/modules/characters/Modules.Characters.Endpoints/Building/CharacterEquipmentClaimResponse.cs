namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>A package Selection and its independent acquisition receipt.</summary>
public sealed record CharacterEquipmentClaimResponse(string SelectionId, string Name, bool Claimed);
