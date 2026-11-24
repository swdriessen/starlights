namespace Starlights.Modules.Identity.Data;

internal sealed class StoredSession
{
    public required string Id { get; set; }
    public required byte[] ProtectedTicket { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
