using FastEndpoints;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.SavingThrows.GetSavingThrows;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Attributes.SavingThrows.GetSavingThrows;

public sealed class GetSavingThrowsEndpoint : EndpointWithoutRequest<GetSavingThrowsResponse>
{
    private readonly IPersistence _persistence;

    public GetSavingThrowsEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/saving-throws");
        Group<ElementsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();

        var savingThrows = await repository.GetElementsByTypeAsync(ElementTypeConstants.SavingThrow);
        var abilities = await repository.GetElementsByTypeAsync(ElementTypeConstants.Ability);
        var abilitiesById = abilities.ToDictionary(x => x.Id, x => x.Name);

        var items = savingThrows
            .Select(st =>
            {
                var abilityId = st.GetRequiredComponent<PrimaryAbilityComponent>().PrimaryAbility;
                abilitiesById.TryGetValue(abilityId, out var abilityName);

                var description = st.GetComponent<DescriptionComponent>()?.Content ?? string.Empty;

                return new SavingThrowListItem(
                    Id: st.Id.Value,
                    Name: st.Name,
                    AbilityId: abilityId.Value,
                    Ability: abilityName ?? string.Empty,
                    Description: description);
            })
            .ToList();

        await Send.OkAsync(new GetSavingThrowsResponse(items), cancellation: ct);
    }
}