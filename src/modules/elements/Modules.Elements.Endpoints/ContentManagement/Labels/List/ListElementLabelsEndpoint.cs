using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Labels.List;

/// <summary>
/// Lists all labels for a given element.
/// </summary>
public sealed class ListElementLabelsEndpoint : EndpointWithoutRequest<ListElementLabelsResponse>
{
    private readonly IPersistence _persistence;

    public ListElementLabelsEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{id:guid}/labels");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var elementId = Route<Guid>("id");
        Logger.LogInformation("listing element labels [id='{Id}']", elementId);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(elementId);

        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var labels = element.GetComponent<ClassificationsComponent>()?.Labels ?? [];
        await Send.OkAsync(new ListElementLabelsResponse(labels.ToArray()), ct);
    }
}
