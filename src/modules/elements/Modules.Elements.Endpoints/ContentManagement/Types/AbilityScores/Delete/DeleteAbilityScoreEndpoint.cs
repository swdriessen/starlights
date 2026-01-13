using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores.Delete;

public sealed class DeleteAbilityScoreEndpoint(IPersistence persistence) : Endpoint<DeleteAbilityScoreRequest>
{
    public override void Configure()
    {
        Delete("/ability-scores/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(DeleteAbilityScoreRequest req, CancellationToken ct)
    {
        Logger.LogInformation("deleting ability score [id='{Id}']", req.Id);

        var repository = persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(req.Id);

        if (element is null || element.Type != ElementTypeConstants.Ability)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var deleted = await repository.DeleteElementAsync(req.Id);
        if (!deleted)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var rows = await persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to delete ability score. No rows affected. [id='{Id}']", req.Id);
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}
