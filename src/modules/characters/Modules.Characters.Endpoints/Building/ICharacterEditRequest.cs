namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>Revision and durable request identity shared by the named build actions.</summary>
public interface ICharacterEditRequest
{
    /// <summary>Gets the revision against which the caller's input was prepared.</summary>
    long ExpectedRevision { get; }
    /// <summary>Gets the unique request identity, retained across dropped-response retries.</summary>
    Guid OperationId { get; }
}
