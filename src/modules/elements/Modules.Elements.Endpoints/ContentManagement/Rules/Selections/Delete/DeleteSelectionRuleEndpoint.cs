using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.Delete;

/// <summary>
/// Deletes a selection rule from an element.
/// </summary>
public sealed class DeleteSelectionRuleEndpoint : EndpointWithoutRequest
{
    private readonly IPersistence _persistence;

    public DeleteSelectionRuleEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Delete("/{elementId:guid}/rules/selections/{ruleId:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var elementId = Route<Guid>("elementId");
        var ruleId = Route<Guid>("ruleId");

        Logger.LogInformation("deleting selection rule [elementId='{ElementId}', ruleId='{RuleId}']", elementId, ruleId);

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

        var removed = element.RemoveComponent<SelectionRuleComponent>(new(ruleId));
        if (!removed)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var rows = await _persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to delete selection rule. No rows affected. [elementId='{ElementId}', ruleId='{RuleId}']", elementId, ruleId);
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}
