namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>Replaces the recorded ability input map, allowing a valid partial standard-array assignment.</summary>
public sealed record AssignCharacterAbilitiesRequest(
    [property: System.Text.Json.Serialization.JsonRequired] long ExpectedRevision,
    Guid OperationId, Dictionary<string, int> Inputs) : ICharacterEditRequest;
