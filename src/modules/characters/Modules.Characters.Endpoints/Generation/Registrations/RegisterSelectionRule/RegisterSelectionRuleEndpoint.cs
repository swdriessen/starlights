using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Services.Processing;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Generation.Registrations.RegisterSelectionRule;

public class RegisterSelectionRuleEndpoint : Endpoint<RegisterSelectionRuleRequest, RegisterSelectionRuleResponse>
{
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;
    private readonly IEnumerable<IRegistrationBehavior> _registrationBehaviors;

    public RegisterSelectionRuleEndpoint(IPersistence persistence, IElementsModuleQueries elements, IEnumerable<IRegistrationBehavior> registrationBehaviors)
    {
        _persistence = persistence;
        _elements = elements;
        _registrationBehaviors = [.. registrationBehaviors];
    }

    public override void Configure()
    {
        Post("{id:guid}/builder/selection-rules/{ruleId:guid}/register");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterSelectionRuleRequest req, CancellationToken ct)
    {
        using var registrationActivity = CharactersInstrumentation.StartActivity(nameof(RegisterSelectionRuleEndpoint));

        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(req.CharacterId);
        if (character is null)
        {
            AddError($"The character '{req.CharacterId}' does not exist.");
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

        var selectionRule = parentRegistration.SelectionRules.FirstOrDefault(r => r.Id == req.SelectionRuleId);
        if (selectionRule is null)
        {
            AddError($"The selection rule '{req.SelectionRuleId}' does not exist on the parent registration '{req.ParentRegistration}'.");
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

        var newRegistration = Registration.Create(character.Id, new(element.Id), element.Name, element.Type);
        newRegistration.UpdateParentRegistration(parentRegistration);

        // mark the selection rule with the selected element
        selectionRule.UpdateCurrentSelection(newRegistration.AssociatedElementId);
        // TODO: add domain event, aka go through parent registration

        registrations.Add(newRegistration);

        // move this to the registration manager? - aka if a class is selected, registration behavior will add the character class entity to the classcomponent
        var context = new RegistrationProcessContext(parentRegistration, _persistence);
        context.NewRegistrations.Add(newRegistration);

        foreach (var behavior in _registrationBehaviors)
        {
            await behavior.Registered(newRegistration, context);
        }

        await _persistence.SaveChangesAsync();

        registrationActivity?.AddTag("registration.id", newRegistration.Id.ToString());
        registrationActivity?.AddTag("registration.name", newRegistration.AssociatedElementName);

        await Send.OkAsync(new RegisterSelectionRuleResponse { RegistrationId = newRegistration.Id }, ct);
    }
}

public class RegisterSelectionRuleResponse
{
    public required Guid RegistrationId { get; set; }
}

public class RegisterSelectionRuleRequest
{
    [BindFrom("id")]
    public required Guid CharacterId { get; set; }

    [BindFrom("parentRegistration")]
    public required Guid ParentRegistration { get; set; }

    [BindFrom("ruleId")]
    public required Guid SelectionRuleId { get; set; }

    [BindFrom("elementId")]
    public required Guid ElementId { get; set; }
}