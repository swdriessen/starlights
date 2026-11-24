using Starlights.Modules.Identity.Features.Session;

namespace Starlights.Modules.Identity.Features;

internal interface IIdentitySessions
{
    Task<bool> SignInAsync(string username, string password, CancellationToken cancellationToken);
    Task SignOutAsync(CancellationToken cancellationToken);
    SessionResponse GetSession();
}
