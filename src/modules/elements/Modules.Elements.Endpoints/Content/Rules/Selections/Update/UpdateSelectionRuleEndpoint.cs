using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.Update;

/// <summary>
/// Updates an existing selection rule on an element.
/// </summary>
public sealed class UpdateSelectionRuleEndpoint : Endpoint<UpdateSelectionRuleRequest, UpdateSelectionRuleResponse>
{
    private readonly IPersistence _persistence;

    public UpdateSelectionRuleEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Put("/{elementId:guid}/rules/selections/{ruleId:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(UpdateSelectionRuleRequest req, CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");
        var ruleId = Route<Guid>("ruleId");

        Logger.LogInformation(
            "updating selection rule [elementId='{ElementId}', ruleId='{RuleId}', displayName='{DisplayName}', type='{Type}']",
            elementId,
            ruleId,
            req.DisplayName,
            req.Type);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(elementId);

        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var component = element.GetComponents<SelectionRuleComponent>()
            .FirstOrDefault(x => x.Id.Value == ruleId);

        if (component is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        component.UpdateElementType(req.Type);
        component.UpdateName(req.DisplayName);
        component.UpdateSupports(req.Supports);
        component.UpdateRangeSupports(req.Range);
        component.UpdateRequirements(req.Requirements);
        component.UpdateQuantity(req.Quantity);
        component.UpdateIsOptional(req.Optional);
        component.UpdateLevelRequirement(req.LevelRequirement);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to update selection rule. No rows affected. [elementId='{ElementId}', ruleId='{RuleId}']", elementId, ruleId);
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.OkAsync(new UpdateSelectionRuleResponse(elementId, ruleId), ct);
    }
}
