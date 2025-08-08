using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Entities.AbilityScores.GetAbilities;

sealed class GetAbilityScoresRequest
{
    [BindFrom("id")]
    public Guid CharacterId { get; set; }
}

sealed class GetAbilityScoresResponse
{
    public List<AbilityScoreItem> AbilityScores { get; set; } = [];
}

sealed class AbilityScoreItem
{
    public Guid AbilityScoreId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Abbreviation { get; init; } = string.Empty;
    public int BaseScore { get; init; }
    public int AdditionalScore { get; init; }
    public int CalculatedScore { get; init; }
    public int CalculatedModifier { get; init; }
}

sealed class GetAbilityScoresEndpoint : Endpoint<GetAbilityScoresRequest, GetAbilityScoresResponse>
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
        var repo = _persistence.GetRepository<ICharactersRepository>();

        var character = await repo.GetCharacterAsync(req.CharacterId) ?? throw new InvalidOperationException($"Character with ID {req.CharacterId} not found.");

        var response = new GetAbilityScoresResponse
        {
            AbilityScores = [.. character.AbilityScores.Select(x => new AbilityScoreItem
            {
                AbilityScoreId = x.Id,
                Name = x.Name,
                Abbreviation = x.Abbreviation,
                BaseScore = x.BaseScore,
                AdditionalScore = x.AdditionalScore,
                CalculatedScore = x.CalculatedScore,
                CalculatedModifier = x.CalculatedModifier
            })]
        };

        await Send.OkAsync(response, ct);
    }
}