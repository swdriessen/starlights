using Microsoft.AspNetCore.SignalR;

namespace Starlights.Modules.Characters.Endpoints.Building;

internal sealed class CharacterBuildNotifications(IHubContext<CharacterNotificationsHub> hub) : ICharacterBuildNotifications
{
    public Task ChangedAsync(Guid ownerId, Guid characterId, long revision, string status)
        => hub.Clients.Group(CharacterNotificationsHub.OwnerGroup(ownerId)).SendAsync("CharacterChanged",
            new { characterId, revision, status }, CancellationToken.None);
}
