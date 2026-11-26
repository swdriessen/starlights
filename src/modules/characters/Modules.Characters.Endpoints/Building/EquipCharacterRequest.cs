namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>Records owned armor and weapon allocations; null clears the corresponding role.</summary>
public sealed record EquipCharacterRequest(
    [property: System.Text.Json.Serialization.JsonRequired] long ExpectedRevision,
    Guid OperationId, string? ArmorId, string? WeaponId) : ICharacterEditRequest;
