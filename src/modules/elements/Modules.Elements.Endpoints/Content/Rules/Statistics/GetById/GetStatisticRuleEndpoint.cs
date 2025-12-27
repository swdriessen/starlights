using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;
using System.Linq;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetById;

/// <summary>
/// Retrieves a statistic rule by id from an existing element.
/// </summary>
public sealed class GetStatisticRuleEndpoint : EndpointWithoutRequest<GetStatisticRuleResponse>
{
    private readonly IPersistence _persistence;

    public GetStatisticRuleEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{elementId:guid}/rules/statistics/{ruleId:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");
        var ruleId = Route<Guid>("ruleId");

        Logger.LogInformation("getting statistic rule [elementId='{ElementId}', ruleId='{RuleId}']", elementId, ruleId);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(elementId);
        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var component = element
            .GetComponents<StatisticRuleComponent>()
            .FirstOrDefault(c => c.Id.Value == ruleId);

        if (component is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = new GetStatisticRuleResponse(
            ElementId: elementId,
            RuleId: ruleId,
            Name: component.Name,
            DisplayName: component.DisplayName,
            Value: component.Value,
            StackingBonus: component.StackingBonus,
            LevelRequirement: component.LevelRequirement,
            Requirements: component.Requirements,
            Minimum: component.Minimum,
            Maximum: component.Maximum,
            OrderSequence: component.OrderSequence);

        await Send.OkAsync(response, ct);
    }
}
