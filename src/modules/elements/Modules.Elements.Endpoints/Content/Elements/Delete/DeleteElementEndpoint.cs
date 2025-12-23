using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Elements.Delete;

/// <summary>
/// Deletes a generic element.
/// </summary>
public sealed class DeleteElementEndpoint : Endpoint<DeleteElementRequest>
{
    private readonly ILogger<DeleteElementEndpoint> _logger;
    private readonly IPersistence _persistence;

    public DeleteElementEndpoint(ILogger<DeleteElementEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Delete("/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(DeleteElementRequest req, CancellationToken ct)
    {
        _logger.LogInformation("deleting element [id='{Id}']", req.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();

        var deleted = await repository.DeleteElementAsync(req.Id);
        if (!deleted)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            _logger.LogError("failed to delete element. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}
