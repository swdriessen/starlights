namespace Starlights.Modules.Characters.Data;

/// <summary>A durable acceptance receipt used to disambiguate a dropped HTTP response.</summary>
public sealed record CharacterAcceptedOperation(Guid OperationId, long AcceptedRevision, string Fingerprint, string Status);
