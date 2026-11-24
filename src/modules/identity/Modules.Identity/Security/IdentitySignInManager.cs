using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Starlights.Modules.Identity.Data;

namespace Starlights.Modules.Identity.Security;

internal sealed class IdentitySignInManager(
    UserManager<ApplicationUser> users,
    IHttpContextAccessor accessor,
    IUserClaimsPrincipalFactory<ApplicationUser> claims,
    IOptions<IdentityOptions> options,
    ILogger<SignInManager<ApplicationUser>> logger,
    IAuthenticationSchemeProvider schemes,
    IUserConfirmation<ApplicationUser> confirmation)
    : SignInManager<ApplicationUser>(users, accessor, claims, options, logger, schemes, confirmation)
{
    public override async Task<bool> CanSignInAsync(ApplicationUser user)
        => !user.IsDisabled && await base.CanSignInAsync(user);
}
