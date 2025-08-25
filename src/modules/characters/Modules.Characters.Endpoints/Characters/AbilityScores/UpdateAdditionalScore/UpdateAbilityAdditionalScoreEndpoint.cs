using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Characters.AbilityScores.UpdateAdditionalScore;

internal sealed class UpdateAbilityAdditionalScoreEndpoint : Endpoint<UpdateAbilityAdditionalScoreRequest, UpdateAbilityAdditionalScoreResponse>
{
    private readonly IPersistence _persistence;

    public UpdateAbilityAdditionalScoreEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/{characterId:guid}/abilities/{abilityScoreId:guid}/additional");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateAbilityAdditionalScoreRequest req, CancellationToken ct)
    {
        var characterId = new CharacterId(Route<Guid>("characterId"));
        var abilityScoreId = new AbilityScoreId(Route<Guid>("abilityScoreId"));

        using var _ = CharactersInstrumentation.StartActivity($"{nameof(UpdateAbilityAdditionalScoreEndpoint)} | {characterId} | {abilityScoreId}");

        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(characterId);
        if (character is null)
        {
            AddError($"Character with ID {characterId} not found.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var ability = character.AbilityScores.SingleOrDefault(a => a.Id == abilityScoreId);
        if (ability is null)
        {
            AddError($"AbilityScore with ID {abilityScoreId} not found for Character {characterId}.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        character.UpdateAbilityAdditionalScore(ability.Id, req.Value);

        await _persistence.SaveChangesAsync();

        var response = new UpdateAbilityAdditionalScoreResponse
        {
            AbilityScoreId = ability.Id,
            BaseScore = ability.BaseScore,
            AdditionalScore = ability.AdditionalScore,
            CalculatedScore = ability.CalculatedScore,
            CalculatedModifier = ability.CalculatedModifier
        };

        await Send.OkAsync(response, ct);
    }
}
