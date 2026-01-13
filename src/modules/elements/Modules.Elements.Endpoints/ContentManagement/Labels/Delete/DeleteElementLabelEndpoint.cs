using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Labels.Delete;

/// <summary>
/// Removes a label from an element.
/// </summary>
public sealed class DeleteElementLabelEndpoint : Endpoint<DeleteElementLabelRequest>
{
    private readonly IPersistence _persistence;

    public DeleteElementLabelEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Delete("/{id:guid}/labels");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(DeleteElementLabelRequest req, CancellationToken ct)
    {
        var elementId = Route<Guid>("id");
        Logger.LogInformation("deleting element labels [id='{Id}', count='{Count}']", elementId, req.Labels.Count);

        if (req.Labels is null || req.Labels.Count == 0)
        {
            AddError(r => r.Labels, "At least one label is required.");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(elementId);

        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var classifications = element.GetComponent<ClassificationsComponent>();
        if (classifications is null)
        {
            await Send.NoContentAsync(ct);
            return;
        }

        foreach (var label in req.Labels)
        {
            classifications.RemoveLabel(label.Trim());
        }

        await _persistence.SaveChangesAsync();

        await Send.NoContentAsync(ct);
    }
}
