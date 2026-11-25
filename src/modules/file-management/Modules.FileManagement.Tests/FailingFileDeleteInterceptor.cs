using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Starlights.Modules.FileManagement.Tests;

internal sealed class FailingFileDeleteInterceptor : DbCommandInterceptor
{
    public bool FailNextDelete { get; set; }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (FailNextDelete && command.CommandText.StartsWith("DELETE FROM", StringComparison.Ordinal))
        {
            FailNextDelete = false;
            throw new DbUpdateException("Simulated metadata deletion failure.");
        }
        return ValueTask.FromResult(result);
    }
}
