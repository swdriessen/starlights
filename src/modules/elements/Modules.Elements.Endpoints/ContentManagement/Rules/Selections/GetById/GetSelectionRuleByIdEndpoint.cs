using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.GetById;

/// <summary>
/// Gets a selection rule by id.
/// </summary>
public sealed class GetSelectionRuleByIdEndpoint : EndpointWithoutRequest<GetSelectionRuleResponse>
{
    private readonly IPersistence _persistence;

    public GetSelectionRuleByIdEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{elementId:guid}/rules/selections/{ruleId:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");
        var ruleId = Route<Guid>("ruleId");

        Logger.LogInformation("getting selection rule [elementId='{ElementId}', ruleId='{RuleId}']", elementId, ruleId);

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

        await Send.OkAsync(
            new GetSelectionRuleResponse(
                component.Id.Value,
                component.Name,
                component.ElementType,
                component.Supports,
                component.RangeSupports,
                component.Quantity,
                component.IsOptional,
                component.LevelRequirement,
                component.Requirements),
            ct);
    }
}
