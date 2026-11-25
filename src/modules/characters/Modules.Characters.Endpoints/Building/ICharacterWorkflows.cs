using Starlights.Modules.Characters.Endpoints.Characters.CreateCharacter;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacterDetails;

namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>Runs owner-scoped creation commands and saved Character queries.</summary>
public interface ICharacterWorkflows
{
    /// <summary>Gets registered content-defined creation options.</summary>
    IReadOnlyList<RulesetOptionResponse> Options { get; }
    /// <summary>Initializes and durably commits a Character.</summary>
    Task<Guid> CreateAsync(CreateCharacterRequest request, CancellationToken cancellationToken);
    /// <summary>Reads a bounded owner-only collection page.</summary>
    Task<GetCharactersResponse> ListAsync(int offset, CancellationToken cancellationToken);
    /// <summary>Reopens a saved Character, returning null when absent or owned by another user.</summary>
    Task<GetCharacterDetailsResponse?> GetAsync(Guid characterId, CancellationToken cancellationToken);
}
