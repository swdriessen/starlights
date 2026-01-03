using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Update;

/// <summary>
/// Updates an existing include rule on an element.
/// </summary>
public sealed class UpdateIncludeRuleEndpoint : Endpoint<UpdateIncludeRuleRequest, UpdateIncludeRuleResponse>
{
    private readonly IPersistence _persistence;

    public UpdateIncludeRuleEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Put("/{elementId:guid}/rules/includes/{ruleId:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(UpdateIncludeRuleRequest req, CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");
        var ruleId = Route<Guid>("ruleId");

        Logger.LogInformation(
            "updating include rule [elementId='{ElementId}', ruleId='{RuleId}', includedElementId='{IncludedElementId}', levelRequirement='{LevelRequirement}']",
            elementId,
            ruleId,
            req.IncludedElementId,
            req.LevelRequirement);

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

        component.UpdateIncludeElement(new ElementId(req.IncludedElementId));
        component.UpdateLevelRequirement(req.LevelRequirement);
        component.UpdateDisplayName(req.DisplayName);
        component.UpdateRequirements(req.RequirementsExpression);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to update include rule. No rows affected. [elementId='{ElementId}', ruleId='{RuleId}']", elementId, ruleId);
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.OkAsync(new UpdateIncludeRuleResponse(elementId, ruleId), ct);
    }
}
