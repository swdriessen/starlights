using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;
using System.Linq;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetList;

/// <summary>
/// Retrieves all statistic rules for a given element.
/// </summary>
public sealed class GetStatisticRulesEndpoint : EndpointWithoutRequest<GetStatisticRulesResponse>
{
    private readonly IPersistence _persistence;

    public GetStatisticRulesEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{elementId:guid}/rules/statistics");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");

        Logger.LogInformation("getting statistic rules [elementId='{ElementId}']", elementId);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(elementId);
        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var rules = element
            .GetComponents<StatisticRuleComponent>()
            .Select(c => new GetStatisticRulesResponse.StatisticRuleItem(
                RuleId: c.Id.Value,
                Name: c.Name,
                DisplayName: c.DisplayName,
                Value: c.Value,
                StackingBonus: c.StackingBonus,
                LevelRequirement: c.LevelRequirement,
                Requirements: c.Requirements,
                OrderSequence: c.OrderSequence))
            .OrderBy(x => x.OrderSequence)
            .ToList();

        await Send.OkAsync(new GetStatisticRulesResponse(elementId, rules), ct);
    }
}
