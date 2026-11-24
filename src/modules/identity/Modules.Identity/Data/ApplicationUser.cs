using Microsoft.AspNetCore.Identity;

namespace Starlights.Modules.Identity.Data;

internal sealed class ApplicationUser : IdentityUser<Guid>
{
    public bool IsDisabled { get; set; }
}
