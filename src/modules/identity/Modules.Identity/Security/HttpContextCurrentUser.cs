using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Starlights.Platform.Hosting;

namespace Starlights.Modules.Identity.Security;

internal sealed class HttpContextCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid? UserId => accessor.HttpContext?.User is { Identity.IsAuthenticated: true } principal
        && Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : null;
}
