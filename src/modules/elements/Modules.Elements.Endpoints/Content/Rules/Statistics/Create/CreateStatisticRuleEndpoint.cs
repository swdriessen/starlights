using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Create;

/// <summary>
/// Adds a new statistic rule to an existing element.
/// </summary>
public sealed class CreateStatisticRuleEndpoint : Endpoint<CreateStatisticRuleRequest, CreateStatisticRuleResponse>
{
    private readonly IPersistence _persistence;

    public CreateStatisticRuleEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/{elementId:guid}/rules/statistics/create");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CreateStatisticRuleRequest req, CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");
        req = req with { ElementId = elementId };

        Logger.LogInformation("creating statistic rule on element [id='{Id}', name='{Name}', value='{Value}']", req.ElementId, req.Name, req.Value);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(req.ElementId);

        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var component = element.AddComponent(id => new StatisticRuleComponent(id, req.Name, req.Value, req.LevelRequirement));

        if (!string.IsNullOrWhiteSpace(req.StackingBonus))
        {
            component.UpdateStackingBonus(req.StackingBonus);
        }

        component.UpdateDisplayName(req.DisplayName);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to create statistic rule. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        var response = new CreateStatisticRuleResponse(req.ElementId, component.Id);
        await Send.CreatedAtAsync("/api/elements/{elementId}/rules/statistics/{ruleId}", new { elementId = response.ElementId, ruleId = response.RuleId }, response, cancellation: ct);
    }
}
