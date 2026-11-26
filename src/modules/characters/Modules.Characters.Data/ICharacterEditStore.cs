using Starlights.Modules.Characters.Domain.Characters;

namespace Starlights.Modules.Characters.Data;

/// <summary>Atomic persistence boundary for revision-checked facts and durable calculation operations.</summary>
public interface ICharacterEditStore
{
    /// <summary>Reads a current owner-scoped aggregate and outcome.</summary>
    Task<CharacterEditState?> ReadAsync(Guid characterId, Guid ownerId, CancellationToken cancellationToken);
    /// <summary>Looks up a previously accepted operation only within its owner's Character.</summary>
    Task<CharacterAcceptedOperation?> FindOperationAsync(Guid characterId, Guid ownerId, Guid operationId, CancellationToken cancellationToken);
    /// <summary>Atomically commits accepted facts and a pending receipt, or rejects a raced revision.</summary>
    Task<bool> AcceptAsync(Character character, long expectedRevision, Guid operationId, string fingerprint, CancellationToken cancellationToken);
    /// <summary>Persists a result or explicit failure, publishing a cache only for its exact revision.</summary>
    Task CompleteAsync(Guid characterId, long revision, Guid operationId, string? sheet, string? failure, CancellationToken cancellationToken);
    /// <summary>Explicitly fails pending operations interrupted by an application restart.</summary>
    Task FailInterruptedAsync(CancellationToken cancellationToken);
}
