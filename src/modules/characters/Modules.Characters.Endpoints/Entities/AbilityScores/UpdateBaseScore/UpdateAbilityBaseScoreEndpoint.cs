using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Endpoints.Entities.AbilityScores;
using Starlights.Platform.Data;
using Starlights.Modules.Characters.Domain;

namespace Starlights.Modules.Characters.Endpoints.Entities.AbilityScores.UpdateBaseScore;

sealed class UpdateAbilityBaseScoreRequest
{
    [BindFrom("characterId")] public Guid CharacterId { get; set; }
    [BindFrom("abilityScoreId")] public Guid AbilityScoreId { get; set; }
    public int Value { get; set; }
}

sealed class UpdateAbilityBaseScoreEndpoint : Endpoint<UpdateAbilityBaseScoreRequest, UpdateAbilityScoreResponse>
{
    private readonly IPersistence _persistence;

    public UpdateAbilityBaseScoreEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/{characterId:guid}/abilities/{abilityScoreId:guid}/base");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateAbilityBaseScoreRequest req, CancellationToken ct)
    {
        using var _ = CharactersInstrumentation.StartActivity($"{nameof(UpdateAbilityBaseScoreEndpoint)} | {req.CharacterId} | {req.AbilityScoreId}");

        var repo = _persistence.GetRepository<ICharactersRepository>();
        var character = await repo.GetCharacterAsync(req.CharacterId) ?? throw new InvalidOperationException($"Character with ID {req.CharacterId} not found.");

        var ability = character.AbilityScores.SingleOrDefault(a => a.Id == new AbilityScoreId(req.AbilityScoreId));
        if (ability is null)
        {
            AddError($"AbilityScore with ID {req.AbilityScoreId} not found for Character {req.CharacterId}.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        ability.UpdateBaseScore(req.Value);
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
