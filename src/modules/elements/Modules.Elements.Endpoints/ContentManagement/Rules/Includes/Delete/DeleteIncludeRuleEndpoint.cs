using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Delete;

/// <summary>
/// Deletes an include rule from an existing element.
/// </summary>
public sealed class DeleteIncludeRuleEndpoint : Endpoint<DeleteIncludeRuleRequest>
{
    private readonly IPersistence _persistence;

    public DeleteIncludeRuleEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Delete("/{elementId:guid}/rules/includes/{ruleId:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(DeleteIncludeRuleRequest req, CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");
        var ruleId = Route<Guid>("ruleId");
        req = req with { ElementId = elementId, RuleId = ruleId };

        Logger.LogInformation("deleting include rule [elementId='{ElementId}', ruleId='{RuleId}']", req.ElementId, req.RuleId);

        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(req.ElementId);

        if (element is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var removed = element.RemoveComponent<IncludeRuleComponent>(new(req.RuleId));
        if (!removed)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to delete include rule. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}
