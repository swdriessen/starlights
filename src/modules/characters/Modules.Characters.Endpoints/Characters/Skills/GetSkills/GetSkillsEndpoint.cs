using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Skills;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Characters.Skills.GetSkills;

internal sealed class GetSkillsEndpoint : EndpointWithoutRequest<GetSkillsResponse>
{
    private readonly IPersistence _persistence;

    public GetSkillsEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{characterId:guid}/skills");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var characterId = new CharacterId(Route<Guid>("characterId"));

        using var _ = CharactersInstrumentation.StartActivity($"{nameof(GetSkillsEndpoint)} | {characterId}");

        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(characterId);
        if (character is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var skillsComponent = character.GetRequiredComponent<SkillsComponent>();

        var response = new GetSkillsResponse
        {
            Skills = [.. skillsComponent.Skills.AsSkillDataModels()]
        };

        await Send.OkAsync(response, ct);
    }
}
