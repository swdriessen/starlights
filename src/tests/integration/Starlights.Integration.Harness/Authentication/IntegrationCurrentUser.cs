using Starlights.Platform.Hosting;

namespace Starlights.Integration.Authentication;

internal sealed class IntegrationCurrentUser : ICurrentUser
{
    public Guid? UserId { get; } = Guid.NewGuid();
}
