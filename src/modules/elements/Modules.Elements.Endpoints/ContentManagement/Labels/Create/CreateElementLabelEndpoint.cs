using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Endpoints.Content.Labels.Create;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Labels.Create;

/// <summary>
/// Adds a label to an element.
/// </summary>
public sealed class CreateElementLabelEndpoint : Endpoint<CreateElementLabelRequest, CreateElementLabelResponse>
{
    private readonly IPersistence _persistence;

    public CreateElementLabelEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/{id:guid}/labels");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CreateElementLabelRequest req, CancellationToken ct)
    {
        var elementId = Route<Guid>("id");
        Logger.LogInformation("adding element labels [id='{Id}', count='{Count}']", elementId, req.Labels.Count);

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
            classifications = element.AddComponent(id => new ClassificationsComponent(id));
        }

        foreach (var label in req.Labels)
        {
            classifications.AddLabel(label.Trim());
        }

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to add label. No rows affected. [id='{Id}']", elementId);
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.CreatedAtAsync(
            $"/api/elements/{elementId}/labels",
            new { id = elementId },
            new CreateElementLabelResponse(classifications.Labels.ToArray()),
            cancellation: ct);
    }
}
