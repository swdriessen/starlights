using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Reorder;

/// <summary>
/// Re-orders rule components of an element (statistic/include/selection) based on an explicit ordered list of rule component ids.
/// </summary>
public sealed class ReorderElementRulesEndpoint : Endpoint<ReorderElementRulesRequest>
{
    private readonly IPersistence _persistence;

    public ReorderElementRulesEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Put("/{elementId:guid}/rules/order");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(ReorderElementRulesRequest req, CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");

        Logger.LogInformation("reordering element rules [elementId='{ElementId}', count='{Count}']", elementId, req.RuleIds.Count);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(elementId);

        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var reordered = ReorderRules(element, req.RuleIds);
        if (!reordered)
        {
            AddError("ruleIds", "rule ids must match existing rules on this element");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("Failed to reorder rules. No rows affected [elementId='{ElementId}']", elementId);
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }

    private static bool ReorderRules(Element element, IReadOnlyList<Guid> orderedRuleIds)
    {
        var existingRuleIds = element.GetComponents<StatisticRuleComponent>().Select(c => c.Id.Value)
            .Concat(element.GetComponents<IncludeRuleComponent>().Select(c => c.Id.Value))
            .Concat(element.GetComponents<SelectionRuleComponent>().Select(c => c.Id.Value))
            .ToList();

        if (existingRuleIds.Count != orderedRuleIds.Count)
        {
            return false;
        }

        var orderedSet = orderedRuleIds.ToHashSet();
        if (existingRuleIds.Any(id => !orderedSet.Contains(id)))
        {
            return false;
        }

        for (var i = 0; i < orderedRuleIds.Count; i++)
        {
            var componentId = new ElementComponentId(orderedRuleIds[i]);
            element.MoveComponent(componentId, i);
        }

        return true;
    }
}
