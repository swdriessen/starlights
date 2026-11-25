using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Platform.Hosting;

namespace Starlights.Modules.Characters.Data.EntityFramework;

internal sealed class CharacterOwnershipInterceptor(ICurrentUser currentUser) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AssignOwners(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AssignOwners(eventData.Context);
        return ValueTask.FromResult(result);
    }

    private void AssignOwners(DbContext? context)
    {
        if (context is null)
        {
            return;
        }
        foreach (var entry in context.ChangeTracker.Entries<Character>().Where(entry => entry.State == EntityState.Added))
        {
            var ownerId = currentUser.UserId
                ?? throw new InvalidOperationException("Creating a Character requires an authenticated owner.");
            if (entry.Entity.RulesetId is not null && entry.Entity.OwnerId != ownerId)
                throw new InvalidOperationException("Character ownership does not match the authenticated owner.");
            entry.Property(character => character.OwnerId).CurrentValue = ownerId;
        }
    }
}
