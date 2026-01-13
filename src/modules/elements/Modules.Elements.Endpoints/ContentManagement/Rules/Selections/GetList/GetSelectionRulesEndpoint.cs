using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.GetList;

/// <summary>
/// Gets all selection rules on an element.
/// </summary>
public sealed class GetSelectionRulesEndpoint : EndpointWithoutRequest<GetSelectionRulesResponse>
{
    private readonly IPersistence _persistence;

    public GetSelectionRulesEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{elementId:guid}/rules/selections");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");

        Logger.LogInformation("getting selection rules [elementId='{ElementId}']", elementId);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(elementId);

        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var rules = element.GetComponents<SelectionRuleComponent>()
            .Select(x => new GetSelectionRulesResponse.SelectionRuleItem(
                x.Id.Value,
                x.Name,
                x.ElementType,
                x.Supports,
                x.RangeSupports,
                x.Quantity,
                x.IsOptional,
                x.LevelRequirement,
                x.Requirements))
            .ToList();

        await Send.OkAsync(new GetSelectionRulesResponse { Rules = rules }, ct);
    }
}
