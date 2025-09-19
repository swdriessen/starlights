using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Appearances;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;

sealed class GetCharactersEndpoint : EndpointWithoutRequest<GetCharactersResponse>
{
    private readonly IPersistence _persistence;

    public GetCharactersEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var _ = CharactersInstrumentation.StartActivity(nameof(GetCharactersEndpoint));

        var repository = _persistence.GetRepository<ICharactersRepository>();

        var characters = await repository.GetCharactersAsync();

        var models = characters.Select(c =>
        {
            return new CharacterDetailsDataModel
            {
                CharacterId = c.Id,
                Name = c.Name,
                PortraitUrl = c.GetRequiredComponent<AppearanceComponent>().PortraitUrl
            };
        }).ToList();

        var response = new GetCharactersResponse { Characters = models };

        await Send.OkAsync(response, ct);
    }
}
