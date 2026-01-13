using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Create;

/// <summary>
/// Adds a new include rule to an existing element.
/// </summary>
public sealed class CreateIncludeRuleEndpoint : Endpoint<CreateIncludeRuleRequest, CreateIncludeRuleResponse>
{
    private readonly IPersistence _persistence;

    public CreateIncludeRuleEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/{elementId:guid}/rules/includes/create");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CreateIncludeRuleRequest req, CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");
        req = req with { ElementId = elementId };

        Logger.LogInformation(
            "creating include rule on element [id='{Id}', includedElementId='{IncludedElementId}']",
            req.ElementId,
            req.IncludedElementId);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(req.ElementId);

        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var component = element.AddComponent(id => new IncludeRuleComponent(id, new ElementId(req.IncludedElementId), req.LevelRequirement));
        component.UpdateDisplayName(req.DisplayName);
        component.UpdateRequirements(req.RequirementsExpression);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to create include rule. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        var response = new CreateIncludeRuleResponse(req.ElementId, component.Id);
        await Send.CreatedAtAsync(
            "/api/elements/{elementId}/rules/includes/{ruleId}",
            new { elementId = response.ElementId, ruleId = response.RuleId },
            response,
            cancellation: ct);
    }
}
