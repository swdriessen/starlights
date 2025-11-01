using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Characters.AbilityScores.GetAbilities;

internal sealed class GetAbilityScoresEndpoint : EndpointWithoutRequest<GetAbilityScoresResponse>
{
    private readonly IPersistence _persistence;

    public GetAbilityScoresEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("/{characterId:guid}/ability-scores");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var characterId = new CharacterId(Route<Guid>("characterId"));

        using var _ = CharactersInstrumentation.StartActivity($"{nameof(GetAbilityScoresEndpoint)} | {characterId}");
        var characters = _persistence.GetRepository<ICharactersRepository>();

        var character = await characters.GetCharacterAsync(characterId);
        if (character is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var abilities = character.GetRequiredComponent<AbilitiesComponent>();

        var response = new GetAbilityScoresResponse { AbilityScores = abilities.AbilityScores.OrderBy(x => x.SortingOrder).AsAbilityScoreDataModels() };

        await Send.OkAsync(response, ct);
    }
}