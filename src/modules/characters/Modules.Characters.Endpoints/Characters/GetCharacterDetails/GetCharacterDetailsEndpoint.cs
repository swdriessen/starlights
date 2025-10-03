using System.Text;
using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Appearances;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Characters.GetCharacterDetails;

sealed class GetCharacterDetailsEndpoint : EndpointWithoutRequest<GetCharacterDetailsResponse>
{
    private readonly IPersistence _persistence;

    public GetCharacterDetailsEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("{characterId:guid}");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var _ = CharactersInstrumentation.StartActivity(nameof(GetCharacterDetailsEndpoint));

        var characterId = new CharacterId(Route<Guid>("characterId"));

        var repository = _persistence.GetRepository<ICharactersRepository>();

        var character = await repository.GetCharacterAsync(characterId);

        if (character is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var appearance = character.GetRequiredComponent<AppearanceComponent>();
        var progression = character.GetRequiredComponent<ProgressionComponent>();
        var classComponent = character.GetRequiredComponent<ClassComponent>();

        var build = new StringBuilder();
        foreach (var item in classComponent.Classes)
        {
            build.Append(item.Name);

            if (classComponent.IsMulticlass)
            {
                build.AppendFormat(" ({0}) /", item.Level);
            }
        }

        var model = new CharacterDetailsDataModel
        {
            CharacterId = character.Id,
            Name = character.Name,
            PortraitUrl = appearance.PortraitUrl,
            Level = progression.CharacterLevel,
            Build = build.ToString().TrimEnd('/').Trim(),
        };

        var response = new GetCharacterDetailsResponse { Character = model };

        await Send.OkAsync(response, ct);
    }
}
