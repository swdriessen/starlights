using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Values;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.AbilityScores.Create;

public sealed class CreateAbilityScoreEndpoint(IPersistence persistence) : Endpoint<CreateAbilityScoreRequest, CreateAbilityScoreResponse>
{
    public override void Configure()
    {
        Post("/ability-scores/create");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CreateAbilityScoreRequest req, CancellationToken ct)
    {
        Logger.LogInformation("creating ability score [name='{Name}', abbr='{Abbreviation}']", req.Name, req.Abbreviation);

        var element = Element.Create(req.Name, ElementTypeConstants.Ability);

        element.AddComponent(id => new DescriptionComponent(id, req.Description ?? string.Empty));
        element.AddComponent(id => new AbbreviationComponent(id, new Abbreviation(req.Abbreviation)));

        var repository = persistence.GetRepository<IElementsRepository>();
        repository.Add(element);

        var rows = await persistence.SaveChangesAsync();
        if (rows == 0)
        {
            Logger.LogError("failed to create ability score. No rows affected.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        var response = new CreateAbilityScoreResponse(element.Id);
        await Send.CreatedAtAsync("/api/elements/ability-scores/{id}", new { id = response.Id }, response, cancellation: ct);
    }
}
