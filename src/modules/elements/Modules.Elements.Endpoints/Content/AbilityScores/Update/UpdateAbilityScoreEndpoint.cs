using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Values;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.AbilityScores.Update;

public sealed class UpdateAbilityScoreEndpoint(IPersistence persistence) : Endpoint<UpdateAbilityScoreRequest, UpdateAbilityScoreResponse>
{
    public override void Configure()
    {
        Put("/ability-scores/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(UpdateAbilityScoreRequest req, CancellationToken ct)
    {
        Logger.LogInformation("updating ability score [id='{Id}']", req.Id);

        if (Route<Guid>("id") != req.Id)
        {
            AddError(r => r.Id, "Route id must match payload id.");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var repository = persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(req.Id);

        if (element is null || element.Type != ElementTypeConstants.Ability)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        element.UpdateName(req.Name);

        element.UpdateComponent<DescriptionComponent>(component =>
        {
            component.UpdateContent(req.Description ?? string.Empty);
        });

        element.UpdateComponent<AbbreviationComponent>(component =>
        {
            component.UpdateAbbreviation(new Abbreviation(req.Abbreviation));
        });

        var rows = await persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to update ability score. No rows affected. [id='{Id}']", req.Id);
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.OkAsync(new UpdateAbilityScoreResponse(element.Id), ct);
    }
}
