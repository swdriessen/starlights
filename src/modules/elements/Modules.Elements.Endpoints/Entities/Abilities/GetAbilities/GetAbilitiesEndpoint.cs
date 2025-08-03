using FastEndpoints;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Endpoints.Entities.Abilities.GetAbilities;

public sealed class GetAbilitiesEndpoint : EndpointWithoutRequest
{
    private readonly IPersistence _persistence;

    public GetAbilitiesEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/abilities");
        AllowAnonymous();
        Group<ElementsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();

        var elements = await repository.GetElementsByTypeAsync(ElementTypeConstants.Ability);

        var abilities = elements.ConvertAll(x => x.AsAbilityInfo());

        await Send.OkAsync(new GetAbilitiesResponse { Abilities = abilities }, cancellation: ct);
    }
}
