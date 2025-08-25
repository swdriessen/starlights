using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Endpoints.Models;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetSelectionRuleOptions;

public sealed class GetSelectionRuleOptionsEndpoint : EndpointWithoutRequest<GetSelectionRuleOptionsResponse>
{
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;

    public GetSelectionRuleOptionsEndpoint(IPersistence persistence, IElementsModuleQueries elements)
    {
        _persistence = persistence;
        _elements = elements;
    }

    public override void Configure()
    {
        Get("{characterId:guid}/builder/selection-rules/{ruleId:guid}/options");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var _ = CharactersInstrumentation.StartActivity(nameof(GetSelectionRuleOptionsEndpoint));

        var characterId = new CharacterId(Route<Guid>("characterId"));
        var ruleId = new RegistrationSelectionRuleId(Route<Guid>("ruleId"));

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

        var selectionRule = characterRegistrations.SelectMany(x => x.SelectionRules)
            .SingleOrDefault(x => x.Id == ruleId);

        if (selectionRule is null)
        {
            AddError($"The selection rule '{ruleId}' does not exist for character '{characterId}'.");
            await Send.NotFoundAsync(cancellation: ct);
            return;
        }

        var elements = await _elements.GetElementsByType(selectionRule.ElementType);

        var response = new GetSelectionRuleOptionsResponse
        {
            Options = elements.ConvertAll(e => new SelectionRuleOptionModel { ElementId = e.Id, Name = e.Name })
        };

        await Send.OkAsync(response, ct);
    }
}
