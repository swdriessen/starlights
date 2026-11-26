using Starlights.Modules.Characters.Domain.Characters;

namespace Starlights.Modules.Characters.Data;

/// <summary>The authoritative aggregate revision and its durable processing outcome.</summary>
public sealed record CharacterEditState(Character Character, long Revision, string Status, Guid? OperationId,
    string? Failure, string? Sheet, long? SheetRevision);
