using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores.GetList;

public sealed class GetAbilityScoresEndpoint(IPersistence persistence) : EndpointWithoutRequest<GetAbilityScoresResponse>
{
    public override void Configure()
    {
        Get("/ability-scores");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Logger.LogInformation("getting ability scores");

        var repository = persistence.GetRepository<IElementsRepository>();
        var elements = await repository.GetElementsByTypeAsync(ElementTypeConstants.Ability);

        var items = elements
            .Select(e => new AbilityScoreDataModel(
                e.Id,
                e.Name,
                e.GetRequiredComponent<AbbreviationComponent>().Abbreviation.Value,
                e.GetComponent<DescriptionComponent>()?.Content ?? string.Empty))
            .ToList();

        await Send.OkAsync(new GetAbilityScoresResponse(items), ct);
    }
}
