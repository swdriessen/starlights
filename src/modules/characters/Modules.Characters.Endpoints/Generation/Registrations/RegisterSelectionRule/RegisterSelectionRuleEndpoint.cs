using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Services.Processing;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Generation.Registrations.RegisterSelectionRule;

public class RegisterSelectionRuleEndpoint : Endpoint<RegisterSelectionRuleRequest, RegisterSelectionRuleResponse>
{
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;
    private readonly IRegistrationManager _registrationManager;

    public RegisterSelectionRuleEndpoint(IPersistence persistence, IElementsModuleQueries elements, IRegistrationManager registrationManager)
    {
        _persistence = persistence;
        _elements = elements;
        _registrationManager = registrationManager;
    }

    public override void Configure()
    {
        Post("{characterId:guid}/builder/selection-rules/{ruleId:guid}/register");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterSelectionRuleRequest req, CancellationToken ct)
    {
        using var registrationActivity = CharactersInstrumentation.StartActivity(nameof(RegisterSelectionRuleEndpoint));

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

        var selectionRule = parentRegistration.SelectionRules.FirstOrDefault(r => r.Id == ruleId);
        if (selectionRule is null)
        {
            AddError($"The selection rule '{ruleId}' does not exist on the parent registration '{req.ParentRegistration}'.");
            await Send.NotFoundAsync(cancellation: ct);
            return;
        }

        var element = await _elements.GetElementWithRules(req.ElementId);
        if (element is null)
        {
            AddError($"The element '{req.ElementId}' does not exist.");
            await Send.NotFoundAsync(cancellation: ct);
            return;
        }


        // create a new registration for the selected element
        var newRegistration = Registration.Create(character.Id, new(element.Id), element.Name, element.Type);
        newRegistration.SetParentRegistration(parentRegistration);
        newRegistration.SetOriginatingRule(selectionRule.Id);

        if (parentRegistration.ProgressionOriginRegistrationId is not null)
        {
            newRegistration.SetProgressionOrigin(parentRegistration.ProgressionOriginRegistrationId.Value);
        }
        else if (string.Equals(parentRegistration.AssociatedElementType, "Class", StringComparison.OrdinalIgnoreCase))
        {
            newRegistration.SetProgressionOrigin(parentRegistration.Id);
        }

        // register the new registration (this will also trigger any registration behaviors)
        await _registrationManager.Register(newRegistration);

        // update the selection rule to point to the new registration
        selectionRule.UpdateCurrentSelection(newRegistration);

        // save changes after all is handled, registration manager itself does not call save changes, nor to the behaviors
        // save changes triggers the processing of the new registration
        await _persistence.SaveChangesAsync();

        registrationActivity?.AddTag("registration.id", newRegistration.Id.ToString());
        registrationActivity?.AddTag("registration.name", newRegistration.AssociatedElementName);

        var response = new RegisterSelectionRuleResponse
        {
            RegistrationId = newRegistration.Id
        };

        await Send.OkAsync(response, ct);
    }
}
