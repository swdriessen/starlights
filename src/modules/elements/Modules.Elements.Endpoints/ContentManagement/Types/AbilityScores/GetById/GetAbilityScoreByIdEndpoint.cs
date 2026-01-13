using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores.GetById;

public sealed class GetAbilityScoreByIdEndpoint(IPersistence persistence) : Endpoint<GetAbilityScoreByIdRequest, AbilityScoreDataModel>
{
    public override void Configure()
    {
        Get("/ability-scores/{id:guid}");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(GetAbilityScoreByIdRequest req, CancellationToken ct)
    {
        var repository = persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(req.Id);

        if (element is null || element.Type != ElementTypeConstants.Ability)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = new AbilityScoreDataModel(
            element.Id,
            element.Name,
            element.GetRequiredComponent<AbbreviationComponent>().Abbreviation.Value,
            element.GetComponent<DescriptionComponent>()?.Content ?? string.Empty);

        await Send.OkAsync(response, ct);
    }
}
