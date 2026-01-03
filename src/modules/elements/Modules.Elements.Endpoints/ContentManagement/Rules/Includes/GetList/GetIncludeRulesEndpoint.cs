using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.GetList;

/// <summary>
/// Retrieves all include rules for a given element.
/// </summary>
public sealed class GetIncludeRulesEndpoint : EndpointWithoutRequest<GetIncludeRulesResponse>
{
    private readonly IPersistence _persistence;

    public GetIncludeRulesEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{elementId:guid}/rules/includes");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");

        Logger.LogInformation("getting include rules [elementId='{ElementId}']", elementId);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(elementId);
        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var rules = element
            .GetComponents<IncludeRuleComponent>()
            .Select(c => new GetIncludeRulesResponse.IncludeRuleItem(
                RuleId: c.Id.Value,
                IncludedElementId: c.IncludeElement.Value,
                LevelRequirement: c.LevelRequirement,
                Requirements: c.Requirements,
                DisplayName: c.DisplayName,
                OrderSequence: c.OrderSequence))
            .OrderBy(x => x.OrderSequence)
            .ToList();

        await Send.OkAsync(new GetIncludeRulesResponse(elementId, rules), ct);
    }
}
