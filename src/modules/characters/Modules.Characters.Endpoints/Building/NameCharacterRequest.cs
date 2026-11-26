namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>Changes a Character's name without changing its identity.</summary>
public sealed record NameCharacterRequest(
    [property: System.Text.Json.Serialization.JsonRequired] long ExpectedRevision,
    Guid OperationId, string Name) : ICharacterEditRequest;
