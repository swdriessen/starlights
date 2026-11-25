using Starlights.Modules.Characters.Domain.Characters;

namespace Starlights.Modules.Characters.Data;

/// <summary>Persists complete initialized aggregates and owner-scoped collection projections.</summary>
public interface ICharacterBuildStore
{
    /// <summary>Commits the root and all initialization state atomically.</summary>
    Task CreateAsync(Character character, CancellationToken cancellationToken);
    /// <summary>Reconstitutes an owner's initialized Character, preserving exact state.</summary>
    Task<Character?> GetAsync(Guid characterId, Guid ownerId, CancellationToken cancellationToken);
    /// <summary>Reads a bounded collection page without requiring Ruleset-specific parts.</summary>
    Task<IReadOnlyList<CharacterHeader>> ListAsync(Guid ownerId, int offset, CancellationToken cancellationToken);
    /// <summary>Checks whether a Character uses the new initialization contract.</summary>
    Task<bool> IsInitializedAsync(Guid characterId, CancellationToken cancellationToken);
}
