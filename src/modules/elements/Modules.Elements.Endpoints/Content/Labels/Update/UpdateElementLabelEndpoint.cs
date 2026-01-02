using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Elements.Labels.Update;

/// <summary>
/// Replaces all labels on an element.
/// </summary>
public sealed class UpdateElementLabelEndpoint : Endpoint<UpdateElementLabelRequest, UpdateElementLabelResponse>
{
    private readonly IPersistence _persistence;

    public UpdateElementLabelEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Put("/{id:guid}/labels");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(UpdateElementLabelRequest req, CancellationToken ct)
    {
        var elementId = Route<Guid>("id");
        Logger.LogInformation("replacing element labels [id='{Id}', count='{Count}']", elementId, req.Labels.Count);

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

        classifications.ReplaceLabels(req.Labels);

        await _persistence.SaveChangesAsync();

        await Send.OkAsync(new UpdateElementLabelResponse(classifications.Labels.ToArray()), ct);
    }
}
