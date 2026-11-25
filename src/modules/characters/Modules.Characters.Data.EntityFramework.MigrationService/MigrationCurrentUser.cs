using Starlights.Platform.Hosting;

namespace Modules.Characters.Data.EntityFramework.MigrationService;

internal sealed class MigrationCurrentUser : ICurrentUser
{
    public Guid? UserId => null;
}
