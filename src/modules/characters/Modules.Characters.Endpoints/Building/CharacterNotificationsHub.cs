using Microsoft.AspNetCore.SignalR;

namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>Authenticated, server-assigned owner groups; clients cannot select an owner or Character group.</summary>
public sealed class CharacterNotificationsHub : Hub
{
    internal static readonly object OwnerKey = new();
    /// <inheritdoc />
    public override async Task OnConnectedAsync()
    {
        // SignalR may execute outside the HTTP accessor's async context (notably Long Polling).
        // The authenticated HTTP boundary captures ICurrentUser into this connection's trusted context.
        var owner = Context.GetHttpContext()?.Items[OwnerKey] as Guid?
            ?? throw new HubException("Authentication is required.");
        await Groups.AddToGroupAsync(Context.ConnectionId, OwnerGroup(owner));
        await base.OnConnectedAsync();
    }
    internal static string OwnerGroup(Guid ownerId) => $"character-owner:{ownerId:N}";
}
