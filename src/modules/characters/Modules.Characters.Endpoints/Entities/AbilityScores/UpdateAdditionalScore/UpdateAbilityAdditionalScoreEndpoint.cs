using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Entities.AbilityScores.UpdateAdditionalScore;

internal sealed class UpdateAbilityAdditionalScoreRequest
{
    [BindFrom("characterId")]
    public Guid CharacterId { get; set; }

    [BindFrom("abilityScoreId")]
    public Guid AbilityScoreId { get; set; }

    public int Value { get; set; }
}

internal sealed class UpdateAbilityAdditionalScoreEndpoint : Endpoint<UpdateAbilityAdditionalScoreRequest, UpdateAbilityScoreResponse>
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

        ability.UpdateAdditionalScore(req.Value);

        // add domain event 
        character.AddDomainEvent(new AbilityScoreUpdatedEvent() { CharacterId = character.Id, AbilityScoreId = ability.Id });


        await _persistence.SaveChangesAsync();

        var response = new UpdateAbilityScoreResponse
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
