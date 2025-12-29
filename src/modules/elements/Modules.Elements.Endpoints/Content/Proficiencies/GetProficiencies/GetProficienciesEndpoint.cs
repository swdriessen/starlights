using FastEndpoints;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Content.Proficiencies.GetProficiencies;

public sealed class GetProficienciesEndpoint : EndpointWithoutRequest<GetProficienciesResponse>
{
    private readonly IPersistence _persistence;

    public GetProficienciesEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/proficiencies");
        Group<ElementsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();

        var proficiencies = await repository.GetElementsByTypeAsync(ElementTypeConstants.Proficiency);

        var items = proficiencies
            .Select(x =>
            {
                var proficiencyType = x.GetComponent<ProficiencyAttributesComponent>()?.ProficiencyType ?? string.Empty;
                var description = x.GetComponent<DescriptionComponent>()?.Content ?? string.Empty;

                return new ProficiencyListItem(
                    Id: x.Id.Value,
                    Name: x.Name,
                    ProficiencyType: proficiencyType,
                    Description: description);
            })
            .ToList();

        await Send.OkAsync(new GetProficienciesResponse(items), cancellation: ct);
    }
}
