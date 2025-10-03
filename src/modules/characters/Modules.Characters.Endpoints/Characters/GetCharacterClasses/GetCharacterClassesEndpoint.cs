using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Characters.GetCharacterClasses;

internal sealed class GetCharacterClassesEndpoint : EndpointWithoutRequest<GetCharacterClassesResponse>
{
    private readonly IPersistence _persistence;

    public GetCharacterClassesEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{characterId:guid}/classes");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var characterId = new CharacterId(Route<Guid>("characterId"));

        using var _ = CharactersInstrumentation.StartActivity($"{nameof(GetCharacterClassesEndpoint)} | {characterId}");

        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(characterId);
        if (character is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var classesComponent = character.GetRequiredComponent<ClassComponent>();

        var response = new GetCharacterClassesResponse
        {
            Classes = classesComponent.Classes.AsCharacterClassDataModels()
        };

        await Send.OkAsync(response, ct);
    }
}
