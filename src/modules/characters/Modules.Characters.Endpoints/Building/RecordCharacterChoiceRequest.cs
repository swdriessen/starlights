namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>Records a complete or partial answer set to an offered Selection.</summary>
public sealed record RecordCharacterChoiceRequest(
    [property: System.Text.Json.Serialization.JsonRequired] long ExpectedRevision,
    Guid OperationId, string SelectionId, string[] Answers) : ICharacterEditRequest;
