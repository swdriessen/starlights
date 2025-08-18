using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Endpoints.Extensions;
using Starlights.Modules.Characters.Endpoints.Entities.Skills;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Entities.Skills.GetSkills;

internal sealed class GetSkillsRequest
{
    [BindFrom("id")]
    public Guid CharacterId { get; set; }
}

internal sealed class GetSkillsResponse
{
    public List<SkillDataModel> Skills { get; set; } = [];
}

internal sealed class GetSkillsEndpoint : Endpoint<GetSkillsRequest, GetSkillsResponse>
{
    private readonly IPersistence _persistence;

    public GetSkillsEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{id:guid}/skills");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetSkillsRequest req, CancellationToken ct)
    {
        using var _ = CharactersInstrumentation.StartActivity($"{nameof(GetSkillsEndpoint)} | {req.CharacterId}");
        var characters = _persistence.GetRepository<ICharactersRepository>();

        var character = await characters.GetCharacterAsync(req.CharacterId);

        if (character is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = new GetSkillsResponse
        {
            Skills = [.. character.Skills.AsSkillDataModels()]
        };

        await Send.OkAsync(response, ct);
    }
}
