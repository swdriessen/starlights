using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.GetById;

/// <summary>
/// Retrieves an include rule by id from an existing element.
/// </summary>
public sealed class GetIncludeRuleEndpoint : EndpointWithoutRequest<GetIncludeRuleResponse>
{
    private readonly IPersistence _persistence;

    public GetIncludeRuleEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{elementId:guid}/rules/includes/{ruleId:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");
        var ruleId = Route<Guid>("ruleId");

        Logger.LogInformation("getting include rule [elementId='{ElementId}', ruleId='{RuleId}']", elementId, ruleId);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(elementId);
        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var component = element
            .GetComponents<IncludeRuleComponent>()
            .FirstOrDefault(c => c.Id.Value == ruleId);

        if (component is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = new GetIncludeRuleResponse(
            ElementId: elementId,
            RuleId: ruleId,
            IncludedElementId: component.IncludeElement.Value,
            LevelRequirement: component.LevelRequirement,
            Requirements: component.Requirements,
            DisplayName: component.DisplayName,
            OrderSequence: component.OrderSequence);

        await Send.OkAsync(response, ct);
    }
}
