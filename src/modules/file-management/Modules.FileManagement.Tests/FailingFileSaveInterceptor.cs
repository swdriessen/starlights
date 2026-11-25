using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Starlights.Modules.FileManagement.Tests;

internal sealed class FailingFileSaveInterceptor : SaveChangesInterceptor
{
    public bool FailNextSave { get; set; }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (FailNextSave)
        {
            FailNextSave = false;
            throw new DbUpdateException("Simulated metadata persistence failure.");
        }
        return ValueTask.FromResult(result);
    }
}
