using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Endpoints.Building;

namespace Starlights.Modules.Characters.Endpoints.Middleware;

internal sealed class CharacterRequestExceptionHandler(ILogger<CharacterRequestExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is CharacterRevisionConflictException)
        {
            logger.LogWarning("Rejected stale or mismatched Character operation");
            await Results.Problem(statusCode: 409, title: "Character revision or operation conflict.",
                detail: "Reload the current Character before reapplying this input. Reuse an operation identity only with its original input.")
                .ExecuteAsync(context);
            return true;
        }
        if (exception is not CharacterRequestException invalid)
            return false;
        logger.LogWarning("Rejected Character request: {Reason}", invalid.Message);
        await Results.Problem(statusCode: 400, title: "Invalid Character request.", detail: invalid.Message)
            .ExecuteAsync(context);
        return true;
    }
}
