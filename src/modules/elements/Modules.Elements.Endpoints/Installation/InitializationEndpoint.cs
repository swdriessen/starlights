using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Integration;

namespace Starlights.Modules.Elements.Endpoints.Installation;

public class InitializationEndpoint : EndpointWithoutRequest
{
    private readonly ILogger<InitializationEndpoint> _logger;
    private readonly IElementsModuleInitializer _initializer;

    public InitializationEndpoint(ILogger<InitializationEndpoint> logger, IElementsModuleInitializer initializer)
    {
        _logger = logger;
        _initializer = initializer;
    }

    public override void Configure()
    {
        Get("/elements/initialize");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        _logger.LogInformation("Initializing Elements module...");

        var result = await _initializer.InitializeAsync();

        var response = new
        {
            Message = "Elements module initialized successfully.",
            ElementsCount = result.NewElementsCount
        };

        await Send.OkAsync(response, ct);
    }
}
