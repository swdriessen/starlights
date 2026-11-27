using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Platform.Hosting;
using Starlights.RuleSets.DND5E5.Building;
using Starlights.RuleSets.DND5E5.Content;

namespace Starlights.Modules.Characters.Endpoints.Building;

internal sealed class CharacterBuildWorkflows(ICurrentUser user, ICharacterEditStore store, CharacterMechanics mechanics,
    ICharacterSheetCalculation calculation, ICharacterBuildNotifications notifications, ILogger<CharacterBuildWorkflows> logger) : ICharacterBuildWorkflows
{
    private Guid OwnerId => user.UserId ?? throw new InvalidOperationException("Authenticated Character access is required.");

    public async Task<CharacterBuildResponse?> GetAsync(Guid characterId, CancellationToken cancellationToken)
    {
        var state = await store.ReadAsync(characterId, OwnerId, cancellationToken);
        return state is null ? null : Snapshot(state);
    }
    public async Task<CharacterBuildResponse?> EditAsync(Guid characterId, ICharacterEditRequest request, CancellationToken cancellationToken)
    {
        using var activity = CharactersInstrumentation.StartActivity("Character.Edit");
        var state = await store.ReadAsync(characterId, OwnerId, cancellationToken);
        if (state is null) return null;
        if (request.ExpectedRevision < 0 || request.ExpectedRevision == long.MaxValue || request.OperationId == Guid.Empty)
            throw new CharacterRequestException("A valid expected revision and operation identity are required.");
        var fingerprint = Fingerprint(request);
        var receipt = await store.FindOperationAsync(characterId, OwnerId, request.OperationId, cancellationToken);
        if (receipt is not null)
        {
            if (receipt.Fingerprint != fingerprint) throw new CharacterRevisionConflictException();
            // Return a fresh authoritative read, but never relabel a later revision as this operation's acceptance.
            return Snapshot((await store.ReadAsync(characterId, OwnerId, cancellationToken))!) with { AcceptedRevision = receipt.AcceptedRevision };
        }
        if (state.Revision != request.ExpectedRevision) throw new CharacterRevisionConflictException();
        var character = state.Character;
        var rules = mechanics.Required(character.Build!.Content);
        try
        {
            switch (request)
            {
                case RecordCharacterChoiceRequest choice when !string.IsNullOrWhiteSpace(choice.SelectionId) && choice.Answers is not null:
                    character.RecordChoice(rules, choice.SelectionId, choice.Answers); break;
                case AssignCharacterAbilitiesRequest abilities when abilities.Inputs is not null:
                    character.AssignAbilityScores(rules, abilities.Inputs); break;
                case ChangeCharacterAbilityGenerationRequest generation when !string.IsNullOrWhiteSpace(generation.Method):
                    var facts = BuildRules.Facts(character.Build!.Composition);
                    if ((generation.Reroll || generation.Method != facts.AbilityGeneration.Method && facts.Inputs.Count > 0)
                        && !generation.ConfirmReplacement)
                        throw new CharacterRequestException("Confirm replacing the current ability assignments or rolled results.");
                    character.ChangeAbilityGeneration(rules, generation.Method, generation.Reroll); break;
                case ClaimCharacterEquipmentRequest claim when !string.IsNullOrWhiteSpace(claim.SelectionId):
                    character.ClaimEquipment(rules, claim.SelectionId); break;
                case EquipCharacterRequest equipment:
                    character.Equip(rules, equipment.ArmorId, equipment.WeaponId); break;
                case NameCharacterRequest name:
                    character.Rename(name.Name); break;
                default: throw new CharacterRequestException("The named action's input is incomplete.");
            }
        }
        catch (ArgumentException exception) when (request is AssignCharacterAbilitiesRequest or ChangeCharacterAbilityGenerationRequest)
        {
            throw new CharacterRequestException(exception.Message);
        }
        catch (ArgumentException) { throw new CharacterRequestException("The named action's input is invalid."); }
        if (!await store.AcceptAsync(character, request.ExpectedRevision, request.OperationId, fingerprint, cancellationToken))
        {
            receipt = await store.FindOperationAsync(characterId, OwnerId, request.OperationId, cancellationToken);
            if (receipt is null || receipt.Fingerprint != fingerprint) throw new CharacterRevisionConflictException();
            return Snapshot((await store.ReadAsync(characterId, OwnerId, cancellationToken))!) with { AcceptedRevision = receipt.AcceptedRevision };
        }
        var revision = request.ExpectedRevision + 1;
        // After the atomic acceptance boundary, browser disconnect/sign-out cannot cancel the calculation or its durable outcome.
        await NotifySafelyAsync(characterId, OwnerId, revision, "pending");
        string? sheet = null, failure = null;
        try { sheet = JsonSerializer.Serialize(Sheet(await calculation.CalculateAsync(rules, character.Build!.Composition),
            BuildRules.Facts(character.Build.Composition))); }
        catch (Exception)
        {
            failure = "Calculation failed. Submit an edit to recalculate.";
            logger.LogWarning("Calculation failed for Character {CharacterId} revision {Revision}", characterId, revision);
        }
        await store.CompleteAsync(characterId, revision, request.OperationId, sheet, failure, CancellationToken.None);
        await NotifySafelyAsync(characterId, OwnerId, revision, failure is null ? "ready" : "failed");
        return Snapshot((await store.ReadAsync(characterId, OwnerId, CancellationToken.None))!) with { AcceptedRevision = revision };
    }

    private async Task NotifySafelyAsync(Guid characterId, Guid ownerId, long revision, string status)
    {
        try { await notifications.ChangedAsync(ownerId, characterId, revision, status); }
        catch (Exception) { logger.LogWarning("Character invalidation delivery failed for {CharacterId} revision {Revision}", characterId, revision); }
    }

    private CharacterBuildResponse Snapshot(CharacterEditState state)
    {
        var build = state.Character.Build!;
        var rules = mechanics.Required(build.Content);
        var view = rules.View(build.Composition);
        var facts = BuildRules.Facts(build.Composition);
        var ready = state.Status == "ready";
        // Rebuild from authoritative facts so older caches cannot hide newly supported fallback/limit evidence.
        var savedSheet = Sheet(view, facts);
        var selections = view.Selections.Select(selection => new CharacterSelectionResponse(selection.Id, selection.Name, selection.Count, selection.Answers,
            selection.Options.Select(option => new CharacterOptionResponse(option.Id, option.Name, option.Description)).ToArray())).ToArray();
        var claims = view.Selections.Where(selection => selection.IsPackage).Select(selection => new CharacterEquipmentClaimResponse(
            selection.Id, selection.Name, facts.Receipts.Any(receipt => receipt.SelectionId == selection.Id)))
            .Concat(facts.Receipts.Where(receipt => !view.Selections.Any(selection => selection.Id == receipt.SelectionId))
                .Select(receipt => new CharacterEquipmentClaimResponse(receipt.SelectionId, rules.Element(receipt.OptionId).Name, true))).ToArray();
        var items = facts.Receipts.SelectMany(receipt => receipt.Items).Select(item =>
        {
            var element = rules.Element(item.ElementId);
            return new CharacterOwnedItemResponse(item.Id, element.Name, item.Quantity,
                element.Property<ArmorProperty>() is not null, element.Property<WeaponProperty>() is not null);
        }).ToArray();
        return new(state.Character.Id, state.Character.Name, state.Revision, state.Status, state.OperationId, state.Failure,
            view.BuildComplete, ready && view.SheetReady, selections,
            view.Abilities.Select(ability => new CharacterAbilityResponse(ability.Id, ability.Name, ability.Input,
                ready ? ability.Score : null, ready ? ability.Modifier : null, ready ? ability.OrdinaryScore : null,
                ability.Maximum, ability.IsFallback, ability.Issue, ability.Sources)).ToArray(), claims, items, facts.ArmorId, facts.WeaponId,
            ready ? savedSheet : null, view.AbilityInputOptions,
            new(facts.AbilityGeneration.Method, facts.AbilityGeneration.Rolls.Select(roll => new CharacterAbilityRollResponse(roll.Dice, roll.Total)).ToArray(),
                view.PointsSpent, view.PointsSpent.HasValue ? 27 - view.PointsSpent.Value : null));
    }

    private static CharacterSheetResponse Sheet(BuildView view, BuildFacts facts) => new(
        view.Statistics.Select(statistic => new CharacterStatisticResponse(statistic.Id, statistic.Name, statistic.Value, statistic.Sources)).ToArray(),
        view.Attacks.Select(attack => new CharacterAttackResponse(attack.Name, attack.ToHit, attack.Damage, attack.Mastery)).ToArray(),
        view.Entries.Select(entry => new CharacterSheetEntryResponse(entry.Name, entry.Description, entry.Usage, entry.Id)).ToArray(),
        facts.Receipts.Sum(receipt => receipt.Gold));

    private static string Fingerprint(ICharacterEditRequest request)
    {
        var raw = JsonSerializer.SerializeToElement(request, request.GetType());
        string Canonical(JsonElement element) => element.ValueKind switch
        {
            JsonValueKind.Object => "{" + string.Join(",", element.EnumerateObject().OrderBy(property => property.Name, StringComparer.Ordinal)
                .Select(property => JsonSerializer.Serialize(property.Name) + ":" + Canonical(property.Value))) + "}",
            JsonValueKind.Array => "[" + string.Join(",", element.EnumerateArray().Select(Canonical)) + "]",
            _ => element.GetRawText()
        };
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.GetType().Name + Canonical(raw))));
    }
}
