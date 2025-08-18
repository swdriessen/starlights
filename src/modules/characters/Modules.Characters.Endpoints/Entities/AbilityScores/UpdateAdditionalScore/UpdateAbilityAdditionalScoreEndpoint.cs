using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Entities.AbilityScores.UpdateAdditionalScore;

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
        using var _ = CharactersInstrumentation.StartActivity($"{nameof(UpdateAbilityAdditionalScoreEndpoint)} | {req.CharacterId} | {req.AbilityScoreId}");

        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(req.CharacterId);
        if (character is null)
        {
            AddError($"Character with ID {req.CharacterId} not found.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var ability = character.AbilityScores.SingleOrDefault(a => a.Id == new AbilityScoreId(req.AbilityScoreId));
        if (ability is null)
        {
            AddError($"AbilityScore with ID {req.AbilityScoreId} not found for Character {req.CharacterId}.");
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
