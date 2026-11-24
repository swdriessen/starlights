using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Identity.Data;
using Starlights.Modules.Identity.Features;

namespace Starlights.Modules.Identity.Accounts;

internal sealed class AccountAdministration(UserManager<ApplicationUser> users, ILogger<AccountAdministration> logger)
    : IAccountAdministration
{
    public async Task<Guid> CreateAsync(string username, string password, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var activity = IdentitySessions.ActivitySource.StartActivity("CreateAccount");
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = username };
        EnsureSucceeded(await users.CreateAsync(user, password));
        logger.LogInformation("Operator created account {UserId}", user.Id);
        return user.Id;
    }

    public async Task ResetPasswordAsync(string username, string password, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var activity = IdentitySessions.ActivitySource.StartActivity("ResetPassword");
        var user = await FindAsync(username);
        var token = await users.GeneratePasswordResetTokenAsync(user);
        EnsureSucceeded(await users.ResetPasswordAsync(user, token, password));
        logger.LogInformation("Operator reset password and revoked sessions for {UserId}", user.Id);
    }

    public async Task SetDisabledAsync(string username, bool disabled, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var activity = IdentitySessions.ActivitySource.StartActivity("SetAccountDisabled");
        var user = await FindAsync(username);
        user.IsDisabled = disabled;
        user.SecurityStamp = Guid.NewGuid().ToString();
        EnsureSucceeded(await users.UpdateAsync(user));
        logger.LogInformation("Operator set account {UserId} disabled to {Disabled} and revoked sessions", user.Id, disabled);
    }

    private async Task<ApplicationUser> FindAsync(string username)
        => await users.FindByNameAsync(username)
            ?? throw new InvalidOperationException("The account does not exist.");

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(error => error.Description)));
        }
    }
}
