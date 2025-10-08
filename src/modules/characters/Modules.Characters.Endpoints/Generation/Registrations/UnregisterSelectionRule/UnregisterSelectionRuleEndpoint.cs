using System.Diagnostics;
using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Services.Processing;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Generation.Registrations.UnregisterSelectionRule;

public class UnregisterSelectionRuleEndpoint : Endpoint<UnregisterSelectionRuleRequest>
{
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;
    private readonly INewRegistrationManager _registrationManager;

    public UnregisterSelectionRuleEndpoint(IPersistence persistence, IElementsModuleQueries elements, INewRegistrationManager registrationManager)
    {
        _persistence = persistence;
        _elements = elements;
        _registrationManager = registrationManager;
    }

    public override void Configure()
    {
        Post("{characterId:guid}/builder/selection-rules/{ruleId:guid}/unregister");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(UnregisterSelectionRuleRequest req, CancellationToken ct)
    {
        using var unregistrationActivity = CharactersInstrumentation.StartActivity(nameof(UnregisterSelectionRuleEndpoint));

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
        var parentRegistration = await registrations.GetRegistrationAsync(new(req.ParentRegistration));
        if (parentRegistration is null)
        {
            AddError($"The parent registration '{req.ParentRegistration}' does not exist.");
            await Send.NotFoundAsync(cancellation: ct);
            return;
        }        

        var selectionRule = parentRegistration.SelectionRules.SingleOrDefault(r => r.Id == ruleId);
        if (selectionRule is null)
        {
            AddError($"The selection rule '{ruleId}' does not exist on the parent registration '{req.ParentRegistration}'.");
            await Send.NotFoundAsync(cancellation: ct);
            return;
        }

        if(!selectionRule.HasCurrentSelection())
        {
            AddError($"The selection rule '{ruleId}' does not have a current selection on the parent registration '{req.ParentRegistration}'.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        // unregister the current selection
        var selectionRegistration = await registrations.GetRegistrationAsync(selectionRule.SelectionRegistrationId.Value);
        await _registrationManager.Unregister(selectionRegistration);

        // clear the selection
        selectionRule.ClearCurrentSelection(); 

        // remove registration when?
        await registrations.DeleteRegistrationAsync(selectionRegistration.Id);

        await _persistence.SaveChangesAsync();

        await Send.OkAsync(cancellation: ct);
    }
}
