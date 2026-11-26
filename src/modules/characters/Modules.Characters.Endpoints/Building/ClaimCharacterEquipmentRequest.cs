namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>Independently claims an already selected starting-equipment package once.</summary>
public sealed record ClaimCharacterEquipmentRequest(
    [property: System.Text.Json.Serialization.JsonRequired] long ExpectedRevision,
    Guid OperationId, string SelectionId) : ICharacterEditRequest;
