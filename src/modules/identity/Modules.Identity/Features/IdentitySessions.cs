using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Identity.Data;
using Starlights.Modules.Identity.Features.Session;
using Starlights.Platform.Hosting;

namespace Starlights.Modules.Identity.Features;

internal sealed class IdentitySessions(
    SignInManager<ApplicationUser> signInManager,
    ICurrentUser currentUser,
    IHttpContextAccessor accessor,
    ILogger<IdentitySessions> logger) : IIdentitySessions
{
    internal static readonly ActivitySource ActivitySource = new("Starlights.Modules.Identity");

    public async Task<bool> SignInAsync(string username, string password, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var activity = ActivitySource.StartActivity("SignIn");
        var result = await signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: true);
        activity?.SetTag("identity.succeeded", result.Succeeded);
        if (result.Succeeded)
        {
            logger.LogInformation("Account signed in");
        }
        else
        {
            logger.LogWarning("Account sign-in rejected");
        }
        return result.Succeeded;
    }

    public async Task SignOutAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var activity = ActivitySource.StartActivity("SignOut");
        await signInManager.SignOutAsync();
        logger.LogInformation("Account signed out");
    }

    public SessionResponse GetSession()
        => new(currentUser.UserId ?? throw new InvalidOperationException("An authenticated user is required."),
            accessor.HttpContext?.User.Identity?.Name
                ?? throw new InvalidOperationException("The authenticated account has no username."));
}
