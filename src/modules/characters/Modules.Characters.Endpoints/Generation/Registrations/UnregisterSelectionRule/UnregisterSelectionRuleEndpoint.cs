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

        if (!selectionRule.HasCurrentSelection())
        {
            AddError($"The selection rule '{ruleId}' does not have a current selection on the parent registration '{req.ParentRegistration}'.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        // TODO: WORKING, BUT MOVE THIS TO THE REGISTRATION MANAGER
        // re-processing needs to unregister...
        // make sure first it is covered by integration tests
        // then refactor

        // Retrieve the registration that is currently selected by the rule.
        var selectionRegistration = await registrations.GetRegistrationAsync(selectionRule.SelectionRegistrationId!.Value);
        if (selectionRegistration is null)
        {
            // Inconsistent state: rule points to non-existing registration.
            AddError($"The selected registration '{selectionRule.SelectionRegistrationId}' no longer exists.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        // Sanity check: ensure the selected registration is actually a child of the parent registration.
        if (selectionRegistration.ParentRegistrationId != parentRegistration.Id)
        {
            AddError($"Selected registration '{selectionRegistration.Id}' is not a child of parent registration '{parentRegistration.Id}'.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        // Load all registrations for the character so we can build the subtree (includes rules & children relationships).
        var allCharacterRegistrations = await registrations.GetRegistrationsAsync(character.Id);

        // Build parent -> children lookup
        var childrenLookup = allCharacterRegistrations
            .Where(r => r.ParentRegistrationId is not null)
            .GroupBy(r => r.ParentRegistrationId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Collect the full subtree (root included)
        var subtree = CollectSubtree(selectionRegistration, childrenLookup);

        // Depth-first removal: unregister children before parents
        // subtree now contains root first (due to stack order). We reverse for leaf-first processing.
        subtree.Reverse();

        foreach (var reg in subtree)
        {
            await _registrationManager.Unregister(reg);
            await registrations.DeleteRegistrationAsync(reg.Id);
        }

        // Clear the selection on the rule now that the associated registration is gone.
        selectionRule.ClearCurrentSelection();

        await _persistence.SaveChangesAsync();

        unregistrationActivity?.AddTag("unregistered.count", subtree.Count.ToString());

        await Send.OkAsync(cancellation: ct);
    }

    private static List<Registration> CollectSubtree(Registration root, Dictionary<RegistrationId, List<Registration>> childrenLookup)
    {
        var collected = new List<Registration> { root };
        var stack = new Stack<Registration>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (childrenLookup.TryGetValue(current.Id, out var children))
            {
                foreach (var child in children)
                {
                    collected.Add(child);
                    stack.Push(child);
                }
            }
        }

        return collected;
    }
}
