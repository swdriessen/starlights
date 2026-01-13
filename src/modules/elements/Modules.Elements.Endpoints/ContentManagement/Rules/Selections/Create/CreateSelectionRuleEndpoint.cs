using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.Create;

/// <summary>
/// Adds a new selection rule to an existing element.
/// </summary>
public sealed class CreateSelectionRuleEndpoint : Endpoint<CreateSelectionRuleRequest, CreateSelectionRuleResponse>
{
    private readonly IPersistence _persistence;

    public CreateSelectionRuleEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/{elementId:guid}/rules/selections/create");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CreateSelectionRuleRequest req, CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");
        req = req with { ElementId = elementId };

        Logger.LogInformation(
            "creating selection rule on element [id='{ElementId}', displayName='{DisplayName}', type='{Type}']",
            req.ElementId,
            req.DisplayName,
            req.Type);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(req.ElementId);

        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var component = element.AddComponent(id => new SelectionRuleComponent(id, req.Type, req.DisplayName, req.LevelRequirement));
        component.UpdateSupports(req.Supports);
        component.UpdateRangeSupports(req.Range);
        component.UpdateRequirements(req.Requirements);
        component.UpdateQuantity(req.Quantity);
        component.UpdateIsOptional(req.Optional);

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to create selection rule. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        var response = new CreateSelectionRuleResponse(req.ElementId, component.Id);
        await Send.CreatedAtAsync(
            "/api/elements/{elementId}/rules/selections/{ruleId}",
            new { elementId = response.ElementId, ruleId = response.RuleId },
            response,
            cancellation: ct);
    }
}
