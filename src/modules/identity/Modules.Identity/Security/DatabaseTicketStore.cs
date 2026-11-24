using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Starlights.Modules.Identity.Data;

namespace Starlights.Modules.Identity.Security;

internal sealed class DatabaseTicketStore(
    IServiceScopeFactory scopes, IDataProtectionProvider protection, TimeProvider time) : ITicketStore
{
    private readonly IDataProtector _protector = protection.CreateProtector("Starlights.Identity.Sessions.v1");

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = Guid.NewGuid().ToString("N");
        using var scope = scopes.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        context.Add(new StoredSession
        {
            Id = key,
            ProtectedTicket = _protector.Protect(TicketSerializer.Default.Serialize(ticket)),
            ExpiresAt = ticket.Properties.ExpiresUtc
                ?? throw new InvalidOperationException("The session must have an expiry.")
        });
        await context.SaveChangesAsync();
        return key;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        using var scope = scopes.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        var session = await context.Set<StoredSession>().FindAsync(key);
        if (session is not null)
        {
            session.ProtectedTicket = _protector.Protect(TicketSerializer.Default.Serialize(ticket));
            session.ExpiresAt = ticket.Properties.ExpiresUtc
                ?? throw new InvalidOperationException("The session must have an expiry.");
            await context.SaveChangesAsync();
        }
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        using var scope = scopes.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        var session = await context.Set<StoredSession>().FindAsync(key);
        if (session is null)
        {
            return null;
        }
        if (session.ExpiresAt <= time.GetUtcNow())
        {
            context.Remove(session);
            await context.SaveChangesAsync();
            return null;
        }
        return TicketSerializer.Default.Deserialize(_protector.Unprotect(session.ProtectedTicket));
    }

    public async Task RemoveAsync(string key)
    {
        using var scope = scopes.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        var session = await context.Set<StoredSession>().FindAsync(key);
        if (session is not null)
        {
            context.Remove(session);
            await context.SaveChangesAsync();
        }
    }
}
