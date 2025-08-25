using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Endpoints.Models;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetSelectionRules;

public sealed class GetSelectionRulesEndpoint : Endpoint<GetSelectionRulesRequest, GetSelectionRulesResponse>
{
    private readonly ILogger<GetSelectionRulesEndpoint> _logger;
    private readonly IPersistence _persistence;

    public GetSelectionRulesEndpoint(ILogger<GetSelectionRulesEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Get("{characterId:guid}/builder/selection-rules");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetSelectionRulesRequest req, CancellationToken ct)
    {
        using var _ = CharactersInstrumentation.StartActivity(nameof(GetSelectionRulesEndpoint));

        var characterId = new CharacterId(Route<Guid>("characterId"));

        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(characterId);
        if (character is null)
        {
            AddError($"The character '{characterId}' does not exist.");
            await Send.NotFoundAsync(cancellation: ct);
            return;
        }

        var registrations = _persistence.GetRepository<IRegistrationRepository>();
        var characterRegistrations = await registrations.GetRegistrationsAsync(character.Id);

        var selectionRules = characterRegistrations.SelectMany(x => x.SelectionRules)
            .Where(x => req.SelectionRuleTypes.Contains(x.ElementType))
            .ToList();

        await Send.OkAsync(new GetSelectionRulesResponse
        {
            Rules = selectionRules.ConvertAll(x => new SelectionRuleDataModel
            {
                RegistrationId = x.ParentRegistrationId,
                RegistrationSelectionRuleId = x.Id,
                Type = x.ElementType,
                Name = x.Name
            })
        }, ct);
    }
}
