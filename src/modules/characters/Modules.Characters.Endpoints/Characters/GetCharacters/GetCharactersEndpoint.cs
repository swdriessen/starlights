using System.Text;
using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Appearances;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Progression;
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

        var models = new List<CharacterDetailsDataModel>();

        foreach (var character in characters)
        {
            var appearance = character.GetRequiredComponent<AppearanceComponent>();
            var progression = character.GetRequiredComponent<ProgressionComponent>();
            var classComponent = character.GetRequiredComponent<ClassComponent>();

            var build = new StringBuilder();
            foreach (var item in classComponent.Classes)
            {
                build.AppendFormat("{0} ({1})", item.Name, item.Level);
            }

            models.Add(new CharacterDetailsDataModel
            {
                CharacterId = character.Id,
                Name = character.Name,
                PortraitUrl = appearance.PortraitUrl,
                Level = progression.CharacterLevel,
                Build = build.ToString()
            });
        }

        var response = new GetCharactersResponse { Characters = models };

        await Send.OkAsync(response, ct);
    }
}
