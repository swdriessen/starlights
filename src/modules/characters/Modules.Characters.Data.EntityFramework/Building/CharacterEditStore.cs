using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Building;
using Starlights.Modules.Characters.Domain.Characters;

namespace Starlights.Modules.Characters.Data.EntityFramework.Building;

internal sealed class CharacterEditStore(IDbContextFactory<CharactersContext> factory, CharacterRulesets rulesets,
    ILogger<CharacterEditStore> logger) : ICharacterEditStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        AllowDuplicateProperties = false,
        RespectRequiredConstructorParameters = true
    };
    public async Task<CharacterEditState?> ReadAsync(Guid characterId, Guid ownerId, CancellationToken cancellationToken)
    {
        // Read the root and state in one database query to avoid a mixed-revision snapshot.
        await using var context = await factory.CreateDbContextAsync(cancellationToken);
        var id = new CharacterId(characterId);
        var state = await context.Set<CharacterBuildRecord>().AsNoTracking().Include(record => record.Character)
            .SingleOrDefaultAsync(record => record.CharacterId == id && record.Character.OwnerId == ownerId, cancellationToken);
        if (state is null) return null;
        var ruleset = rulesets.GetRequired(state.Character.RulesetId!);
        var composition = JsonSerializer.Deserialize(state.Composition, ruleset.CompositionType, JsonOptions) as ICharacterComposition
            ?? throw new InvalidOperationException("The persisted Character composition is unavailable.");
        var character = Character.Reconstitute(id, ownerId, state.Character.Name,
            new(ruleset, new(state.PackId, state.PackVersion, state.CharacterElementId), composition));
        return new(character, state.Revision, state.Status, state.OperationId, state.Failure, state.Sheet, state.SheetRevision);
    }

    public async Task<CharacterAcceptedOperation?> FindOperationAsync(Guid characterId, Guid ownerId, Guid operationId, CancellationToken cancellationToken)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken);
        var id = new CharacterId(characterId);
        return await (from operation in context.Set<CharacterEditOperationRecord>()
                      join character in context.Set<Character>() on operation.CharacterId equals character.Id
                      where operation.CharacterId == id && operation.OperationId == operationId && character.OwnerId == ownerId
                      select new CharacterAcceptedOperation(operation.OperationId, operation.Revision, operation.Fingerprint, operation.Status))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> AcceptAsync(Character character, long expectedRevision, Guid operationId, string fingerprint, CancellationToken cancellationToken)
    {
        using var activity = CharactersInstrumentation.StartActivity("Character.Persistence.AcceptEdit");
        await using var context = await factory.CreateDbContextAsync(cancellationToken);
        var state = await context.Set<CharacterBuildRecord>().Include(record => record.Character)
            .SingleOrDefaultAsync(record => record.CharacterId == character.Id && record.Character.OwnerId == character.OwnerId, cancellationToken);
        if (state is null || state.Revision != expectedRevision) return false;
        var build = character.Build!;
        rulesets.GetRequired(build.RulesetId).Validate(build.Composition);
        state.Composition = JsonSerializer.Serialize(build.Composition, build.Composition.GetType());
        context.Entry(state.Character).Property(root => root.Name).CurrentValue = character.Name;
        state.Revision = checked(expectedRevision + 1);
        state.Status = "pending";
        state.OperationId = operationId;
        state.Failure = null;
        state.Sheet = null;
        state.SheetRevision = null;
        context.Add(new CharacterEditOperationRecord
        {
            CharacterId = character.Id, OperationId = operationId, Revision = state.Revision, Fingerprint = fingerprint
        });
        try
        {
            // EF's transaction includes both the revision-guarded update and the receipt.
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException) { return false; }
        catch (DbUpdateException)
        {
            // A unique operation/revision race must be reported as conflict, not overwrite.
            await using var check = await factory.CreateDbContextAsync(cancellationToken);
            if (await check.Set<CharacterEditOperationRecord>().AnyAsync(record => record.CharacterId == character.Id
                && (record.OperationId == operationId || record.Revision == expectedRevision + 1), cancellationToken)) return false;
            throw;
        }
        logger.LogInformation("Accepted Character {CharacterId} revision {Revision} operation {OperationId}", character.Id, state.Revision, operationId);
        return true;
    }

    public async Task CompleteAsync(Guid characterId, long revision, Guid operationId, string? sheet, string? failure, CancellationToken cancellationToken)
    {
        using var activity = CharactersInstrumentation.StartActivity("Character.Persistence.CompleteEdit");
        await using var context = await factory.CreateDbContextAsync(cancellationToken);
        var id = new CharacterId(characterId);
        var operation = await context.Set<CharacterEditOperationRecord>()
            .SingleAsync(record => record.CharacterId == id && record.OperationId == operationId, cancellationToken);
        if (operation.Status != "pending") return;
        var state = await context.Set<CharacterBuildRecord>().SingleAsync(record => record.CharacterId == id, cancellationToken);
        operation.Status = failure is null ? "ready" : "failed";
        operation.Sheet = sheet;
        operation.Failure = failure;
        if (state.Revision == revision && state.OperationId == operationId)
        {
            state.Status = operation.Status;
            state.Failure = failure;
            state.Sheet = sheet;
            state.SheetRevision = failure is null ? revision : null;
        }
        try { await context.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateConcurrencyException)
        {
            // A later edit won; retain this operation's outcome without publishing its obsolete cache.
            context.ChangeTracker.Clear();
            operation = await context.Set<CharacterEditOperationRecord>().SingleAsync(
                record => record.CharacterId == id && record.OperationId == operationId, cancellationToken);
            if (operation.Status != "pending") return;
            operation.Status = failure is null ? "ready" : "failed";
            operation.Sheet = sheet;
            operation.Failure = failure;
            await context.SaveChangesAsync(cancellationToken);
        }
        logger.LogInformation("Completed Character {CharacterId} revision {Revision} with status {Status}", characterId, revision, operation.Status);
    }

    public async Task FailInterruptedAsync(CancellationToken cancellationToken)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken);
        var pending = await context.Set<CharacterEditOperationRecord>().Where(record => record.Status == "pending").ToListAsync(cancellationToken);
        foreach (var operation in pending)
        {
            operation.Status = "failed";
            operation.Failure = "Calculation was interrupted by an application restart. Submit an edit to recalculate.";
        }
        var buildsPending = await context.Set<CharacterBuildRecord>().Where(record => record.Status == "pending").ToListAsync(cancellationToken);
        foreach (var state in buildsPending)
        {
            state.Status = "failed";
            state.Failure = "Calculation was interrupted by an application restart. Submit an edit to recalculate.";
            state.Sheet = null;
            state.SheetRevision = null;
        }
        await context.SaveChangesAsync(cancellationToken);
        if (pending.Count > 0) logger.LogWarning("Explicitly failed {OperationCount} interrupted Character calculations", pending.Count);
    }
}
