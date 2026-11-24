namespace Starlights.Modules.Characters.Data;

/// <summary>Checks ownership without exposing Character persistence to other modules.</summary>
public interface ICharacterOwnership
{
    /// <summary>Determines whether an account owns the requested Character.</summary>
    Task<bool> IsOwnerAsync(Guid characterId, Guid userId, CancellationToken cancellationToken);
}
