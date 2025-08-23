using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Endpoints.Models;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetSelectionRuleOptions;

public sealed class GetSelectionRuleOptionsEndpoint : Endpoint<GetSelectionRuleOptionsRequest, GetSelectionRuleOptionsResponse>
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
        Get("{characterId:guid}/builder/selection-rules/{selectionRuleId:guid}/options");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetSelectionRuleOptionsRequest req, CancellationToken ct)
    {
        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(req.CharacterId);
        if (character is null)
        {
            AddError($"The character '{req.CharacterId}' does not exist.");
            await Send.NotFoundAsync(cancellation: ct);
            return;
        }

        var registrations = _persistence.GetRepository<IRegistrationRepository>();
        var characterRegistrations = await registrations.GetRegistrationsAsync(character.Id);

        var selectionRule = characterRegistrations.SelectMany(x => x.SelectionRules)
            .SingleOrDefault(x => x.Id == req.SelectionRuleId);

        if (selectionRule is null)
        {
            AddError($"The selection rule '{req.SelectionRuleId}' does not exist for character '{req.CharacterId}'.");
            await Send.NotFoundAsync(cancellation: ct);
            return;
        }

        var elements = await _elements.GetElementsByType(selectionRule.ElementType);

        await Send.OkAsync(new GetSelectionRuleOptionsResponse
        {
            Options = [.. elements.Select(e => new SelectionRuleOptionModel { ElementId = e.Id, Name = e.Name })]
        }, ct);
    }
}

public class GetSelectionRuleOptionsRequest
{
    public Guid CharacterId { get; set; }
    public Guid SelectionRuleId { get; set; }
}

public class GetSelectionRuleOptionsResponse
{
    public List<SelectionRuleOptionModel> Options { get; set; } = [];
}