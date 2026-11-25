using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Building;
using Starlights.Modules.Characters.Domain.Characters;

namespace Starlights.Modules.Characters.Data.EntityFramework.Building;

internal sealed class CharacterBuildStore(
    IDbContextFactory<CharactersContext> factory,
    CharacterRulesets rulesets,
    ILogger<CharacterBuildStore> logger) : ICharacterBuildStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        AllowDuplicateProperties = false,
        RespectRequiredConstructorParameters = true
    };

    public async Task CreateAsync(Character character, CancellationToken cancellationToken)
    {
        using var activity = CharactersInstrumentation.StartActivity("Character.Persistence.Create");
        var build = character.Build ?? throw new InvalidOperationException("Initialized Character state is required.");
        var ruleset = rulesets.GetRequired(build.RulesetId);
        ruleset.Validate(build.Composition);
        await using var context = await factory.CreateDbContextAsync(cancellationToken);
        context.Add(character);
        context.Add(new CharacterBuildRecord
        {
            CharacterId = character.Id,
            Character = character,
            PackId = build.Content.PackId,
            PackVersion = build.Content.PackVersion,
            CharacterElementId = build.Content.CharacterElementId,
            Composition = JsonSerializer.Serialize(build.Composition, ruleset.CompositionType, JsonOptions)
        });
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Committed Character {CharacterId} with Ruleset {RulesetId}", character.Id, build.RulesetId);
    }

    public async Task<Character?> GetAsync(Guid characterId, Guid ownerId, CancellationToken cancellationToken)
    {
        using var activity = CharactersInstrumentation.StartActivity("Character.Persistence.Reconstitute");
        await using var context = await factory.CreateDbContextAsync(cancellationToken);
        var id = new CharacterId(characterId);
        var root = await context.Set<Character>().AsNoTracking()
            .SingleOrDefaultAsync(character => character.Id == id && character.OwnerId == ownerId, cancellationToken);
        if (root is null || root.RulesetId is null)
            return null;
        var state = await context.Set<CharacterBuildRecord>().AsNoTracking()
            .SingleOrDefaultAsync(record => record.CharacterId == id, cancellationToken)
            ?? throw new InvalidOperationException("Saved Character initialization state is missing.");
        var ruleset = rulesets.GetRequired(root.RulesetId);
        if (root.RulesetId != ruleset.Id)
            throw new InvalidOperationException("Saved Character Ruleset is not canonical.");
        var composition = JsonSerializer.Deserialize(state.Composition, ruleset.CompositionType, JsonOptions) as ICharacterComposition
            ?? throw new InvalidOperationException("Saved Character composition is invalid.");
        var binding = new CharacterContentBinding(state.PackId, state.PackVersion, state.CharacterElementId);
        var restored = Character.Reconstitute(root.Id, root.OwnerId, root.Name, new(ruleset, binding, composition));
        logger.LogInformation("Reconstituted Character {CharacterId} with Ruleset {RulesetId}", root.Id, ruleset.Id);
        return restored;
    }

    public async Task<IReadOnlyList<CharacterHeader>> ListAsync(Guid ownerId, int offset, CancellationToken cancellationToken)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken);
        return await context.Set<Character>().AsNoTracking()
            .Where(character => character.OwnerId == ownerId)
            .OrderBy(character => (Guid)character.Id).Skip(offset).Take(51)
            .Select(character => new CharacterHeader(character.Id.Value, character.Name, character.RulesetId))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsInitializedAsync(Guid characterId, CancellationToken cancellationToken)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken);
        return await context.Set<Character>().AnyAsync(
            character => character.Id == new CharacterId(characterId) && character.RulesetId != null, cancellationToken);
    }
}
