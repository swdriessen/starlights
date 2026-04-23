using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Starlights.Platform.Components.FastEndpoints;

internal sealed class OperationCancelledFilter : IEndpointFilter
{
    private readonly ILogger<OperationCancelledFilter> _logger;

    public OperationCancelledFilter(ILogger<OperationCancelledFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            return await next(context);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogDebug(ex, "The endpoint request was cancelled!");
            return Results.StatusCode(499);
        }
    }
}
