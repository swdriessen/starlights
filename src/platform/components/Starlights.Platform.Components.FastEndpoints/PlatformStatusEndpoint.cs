using FastEndpoints;

namespace Starlights.Platform.Components.FastEndpoints;

/// <summary>
/// Endpoint that provides the status of the platform.
/// </summary>
/// <remarks>
/// This is a first endpoint to test the FastEndpoints integration. Without at least one endpoint, the FastEndpoints component would not be registered correctly.
/// </remarks>
public class PlatformStatusEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/platform/status");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        await Send.OkAsync(new { Message = "The platform is operational.", Timestamp = DateTime.UtcNow }, cancellationToken);
    }
}
