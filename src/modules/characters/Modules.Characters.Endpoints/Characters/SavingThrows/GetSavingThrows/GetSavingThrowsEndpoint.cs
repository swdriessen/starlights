using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.SavingThrows;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Characters.SavingThrows.GetSavingThrows;

internal sealed class GetSavingThrowsEndpoint : EndpointWithoutRequest<GetSavingThrowsResponse>
{
    private readonly IPersistence _persistence;

    public GetSavingThrowsEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{characterId:guid}/saving-throws");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var characterId = new CharacterId(Route<Guid>("characterId"));

        using var _ = CharactersInstrumentation.StartActivity($"{nameof(GetSavingThrowsEndpoint)} | {characterId}");
        var characters = _persistence.GetRepository<ICharactersRepository>();

        var character = await characters.GetCharacterAsync(characterId);
        if (character is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var savingThrowsComponent = character.GetRequiredComponent<SavingThrowsComponent>();

        var response = new GetSavingThrowsResponse { SavingThrows = savingThrowsComponent.SavingThrows.AsSavingThrowDataModels() };

        await Send.OkAsync(response, ct);
    }
}
