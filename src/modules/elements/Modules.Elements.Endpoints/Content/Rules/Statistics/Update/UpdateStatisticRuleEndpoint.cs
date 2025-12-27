using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Update;

/// <summary>
/// Updates an existing statistic rule on an existing element.
/// </summary>
public sealed class UpdateStatisticRuleEndpoint : Endpoint<UpdateStatisticRuleRequest, UpdateStatisticRuleResponse>
{
    private readonly IPersistence _persistence;

    public UpdateStatisticRuleEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Put("/{elementId:guid}/rules/statistics/{ruleId:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(UpdateStatisticRuleRequest req, CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");
        var ruleId = Route<Guid>("ruleId");

        Logger.LogInformation(
            "updating statistic rule [elementId='{ElementId}', ruleId='{RuleId}', name='{Name}', value='{Value}']",
            elementId,
            ruleId,
            req.Name,
            req.Value);

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

        component.UpdateName(req.Name);
        component.UpdateValue(req.Value);
        component.UpdateStackingBonus(req.StackingBonus);
        component.UpdateLevelRequirement(req.LevelRequirement);
        component.UpdateDisplayName(req.DisplayName);
        component.UpdateMinimum(req.Minimum);
        component.UpdateMaximum(req.Maximum);
        component.UpdateRequirements(req.RequirementsExpression);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to update statistic rule. No rows affected. [elementId='{ElementId}', ruleId='{RuleId}']", elementId, ruleId);
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.OkAsync(new UpdateStatisticRuleResponse(elementId, ruleId), ct);
    }
}
