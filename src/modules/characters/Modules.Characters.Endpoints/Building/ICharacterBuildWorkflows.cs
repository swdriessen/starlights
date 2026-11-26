namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>Direct asynchronous application workflows for authoritative Character build edits.</summary>
public interface ICharacterBuildWorkflows
{
    /// <summary>Reads the authenticated owner's revision-qualified snapshot.</summary>
    Task<CharacterBuildResponse?> GetAsync(Guid characterId, CancellationToken cancellationToken);
    /// <summary>Validates, commits and finishes a named operation independently of post-acceptance request cancellation.</summary>
    Task<CharacterBuildResponse?> EditAsync(Guid characterId, ICharacterEditRequest request, CancellationToken cancellationToken);
}
