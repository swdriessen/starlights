namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>Replaces the recorded input map with valid partial or complete assignments for the saved generation method.</summary>
public sealed record AssignCharacterAbilitiesRequest(
    [property: System.Text.Json.Serialization.JsonRequired] long ExpectedRevision,
    Guid OperationId, Dictionary<string, int> Inputs) : ICharacterEditRequest;
