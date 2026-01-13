using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Delete;

/// <summary>
/// Deletes one or more rule components (statistic/include/selection) from an element.
/// </summary>
public sealed class DeleteElementRulesEndpoint : Endpoint<DeleteElementRulesRequest>
{
    private readonly IPersistence _persistence;

    public DeleteElementRulesEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Delete("/{elementId:guid}/rules");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(DeleteElementRulesRequest req, CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");
        req = req with { ElementId = elementId };

        Logger.LogInformation(
            "deleting element rules [elementId='{ElementId}', count='{Count}']",
            req.ElementId,
            req.RuleIds.Count);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(req.ElementId);

        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        for (var i = 0; i < req.RuleIds.Count; i++)
        {
            var removed = element.RemoveComponent(new ElementComponentId(req.RuleIds[i]));
            if (!removed)
            {
                await Send.NotFoundAsync(ct);
                return;
            }
        }

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to delete element rules. No rows affected. [elementId='{ElementId}']", req.ElementId);
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}
