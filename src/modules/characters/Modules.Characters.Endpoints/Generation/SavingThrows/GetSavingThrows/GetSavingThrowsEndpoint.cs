using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Endpoints.Extensions;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Generation.SavingThrows.GetSavingThrows;

internal sealed class GetSavingThrowsEndpoint : Endpoint<GetSavingThrowsRequest, GetSavingThrowsResponse>
{
    private readonly IPersistence _persistence;

    public GetSavingThrowsEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{id:guid}/saving-throws");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetSavingThrowsRequest req, CancellationToken ct)
    {
        using var _ = CharactersInstrumentation.StartActivity($"{nameof(GetSavingThrowsEndpoint)} | {req.CharacterId}");
        var characters = _persistence.GetRepository<ICharactersRepository>();

        var character = await characters.GetCharacterAsync(req.CharacterId);

        if (character is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = new GetSavingThrowsResponse
        {
            SavingThrows = [.. character.SavingThrows.AsSavingThrowDataModels()]
        };

        await Send.OkAsync(response, ct);
    }
}
