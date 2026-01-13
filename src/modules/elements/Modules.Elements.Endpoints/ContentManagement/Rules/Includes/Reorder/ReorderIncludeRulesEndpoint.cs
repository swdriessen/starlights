using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Reorder;

/// <summary>
/// Reorders the include rules of an element.
/// </summary>
public sealed class ReorderIncludeRulesEndpoint : Endpoint<ReorderIncludeRulesRequest>
{
    private readonly IPersistence _persistence;

    public ReorderIncludeRulesEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Put("/{elementId:guid}/rules/includes/reorder");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(ReorderIncludeRulesRequest req, CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");
        req = req with { ElementId = elementId };

        Logger.LogInformation("reordering include rules [elementId='{ElementId}', count='{Count}']", req.ElementId, req.RuleIds.Count);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(req.ElementId);

        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var includeRules = element.GetComponents<IncludeRuleComponent>().ToList();

        if (includeRules.Count != req.RuleIds.Count)
        {
            AddError("RuleIds", "RuleIds must contain exactly all include rule ids for this element.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var expectedIds = includeRules.Select(x => x.Id.Value).ToHashSet();
        if (!req.RuleIds.All(expectedIds.Contains))
        {
            AddError("RuleIds", "RuleIds must contain exactly all include rule ids for this element.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        for (var i = 0; i < req.RuleIds.Count; i++)
        {
            element.MoveComponent(new(req.RuleIds[i]), i);
        }

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to reorder include rules. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}
