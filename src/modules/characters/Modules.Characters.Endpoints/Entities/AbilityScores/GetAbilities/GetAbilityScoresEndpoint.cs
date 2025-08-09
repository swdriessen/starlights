using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Endpoints.Extensions;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Entities.AbilityScores.GetAbilities;

internal sealed class GetAbilityScoresRequest
{
    [BindFrom("id")]
    public Guid CharacterId { get; set; }
}

internal sealed class GetAbilityScoresResponse
{
    public List<AbilityScoreDataModel> AbilityScores { get; set; } = [];
}

internal sealed class GetAbilityScoresEndpoint : Endpoint<GetAbilityScoresRequest, GetAbilityScoresResponse>
{
    private readonly IPersistence _persistence;

    public GetAbilityScoresEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{id:guid}/abilities");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAbilityScoresRequest req, CancellationToken ct)
    {
        using var _ = CharactersInstrumentation.StartActivity($"{nameof(GetAbilityScoresEndpoint)} | {req.CharacterId}");
        var characters = _persistence.GetRepository<ICharactersRepository>();

        var character = await characters.GetCharacterAsync(req.CharacterId);

        if (character is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = new GetAbilityScoresResponse
        {
            AbilityScores = [.. character.AbilityScores.AsAbilityScoreDataModels()]
        };

        await Send.OkAsync(response, ct);
    }
}