using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Elements.Update;

/// <summary>
/// Updates a generic element.
/// </summary>
public sealed class UpdateElementEndpoint : Endpoint<UpdateElementRequest, UpdateElementResponse>
{
    private readonly ILogger<UpdateElementEndpoint> _logger;
    private readonly IPersistence _persistence;

    public UpdateElementEndpoint(ILogger<UpdateElementEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Put("/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(UpdateElementRequest req, CancellationToken ct)
    {
        _logger.LogInformation("updating element [id='{Id}']", req.Id);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(req.Id);

        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        element.UpdateName(req.Name);

        var description = element.GetComponent<DescriptionComponent>();

        if (req.Description is null)
        {
            // Keep current behavior: if description isn't supplied, do not modify it.
        }
        else if (description is null)
        {
            element.AddComponent(id => new DescriptionComponent(id, req.Description));
        }
        else
        {
            element.UpdateComponent<DescriptionComponent>(c => c.UpdateContent(req.Description));
        }

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            _logger.LogError("failed to update element. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.OkAsync(new UpdateElementResponse(element.Id), ct);
    }
}
