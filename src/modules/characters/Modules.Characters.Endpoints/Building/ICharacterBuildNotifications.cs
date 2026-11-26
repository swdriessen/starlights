namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>Owner-scoped invalidations; HTTP remains the source of authoritative Character data.</summary>
public interface ICharacterBuildNotifications
{
    /// <summary>Invalidates an owner's clients without transmitting private build facts.</summary>
    Task ChangedAsync(Guid ownerId, Guid characterId, long revision, string status);
}
