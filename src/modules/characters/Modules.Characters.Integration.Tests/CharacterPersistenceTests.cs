using System.Data.Common;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Starlights.Modules.Characters.Data.EntityFramework;
using Starlights.Modules.Characters.Data.EntityFramework.Building;
using Starlights.Modules.Characters.Domain.Building;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.RuleSets.DND5E5.Building;
using Starlights.RuleSets.DND5E5.Rules;
using Starlights.RuleSets.TEST;

[assembly: DoNotParallelize]

namespace Starlights.Modules.Characters.Integration.Tests;

[TestClass]
public sealed class CharacterPersistenceTests(TestContext testContext)
{
    private string _directory = null!;
    private DbContextOptions<CharactersContext> _options = null!;
    private readonly FailingInsertInterceptor _failure = new();

    [TestInitialize]
    public async Task Initialize()
    {
        _directory = Path.Combine(Directory.GetCurrentDirectory(), ".agent-work", $"CharacterPersistence.{Guid.NewGuid():N}");
        Directory.CreateDirectory(_directory);
        _options = new DbContextOptionsBuilder<CharactersContext>()
            .UseSqlite($"Data Source={Path.Combine(_directory, "characters.db")};Pooling=False")
            .AddInterceptors(_failure).Options;
        await using var context = new CharactersContext(_options);
        await context.Database.EnsureCreatedAsync(testContext.CancellationToken);
    }

    [TestCleanup]
    public void Cleanup() => Directory.Delete(_directory, recursive: true);

    [TestMethod]
    [DataRow("DND5E5")]
    [DataRow("TEST")]
    [DataRow("both")]
    [DataRow("reversed")]
    public async Task FreshContexts_PreserveEachRulesetsAuthoritativeState(string configuration)
    {
        ICharacterRuleset[] modules = configuration switch
        {
            "DND5E5" => [new Ruleset()],
            "TEST" => [new TestRuleset()],
            "both" => [new Ruleset(), new TestRuleset()],
            _ => [new TestRuleset(), new Ruleset()]
        };
        var registry = new CharacterRulesets(modules);
        var store = Store(registry);
        var owner = Guid.NewGuid();
        foreach (var module in modules)
        {
            var content = new CharacterContentBinding("fixture", "1.2.3", "fixture:character");
            var created = Character.Create(owner, "Mira", new(module, content, module.Initialize(content)));
            await store.CreateAsync(created, CancellationToken.None);
            var loaded = await Store(registry).GetAsync(created.Id, owner, CancellationToken.None);
            loaded.Should().NotBeNull();
            loaded!.Id.Should().Be(created.Id);
            loaded.OwnerId.Should().Be(owner);
            loaded.Name.Should().Be("Mira");
            loaded.Build!.Content.Should().Be(content);
            loaded.Build.RulesetId.Should().Be(module.Id);
            if (module.Id == "TEST")
                loaded.Build.Composition.Should().BeOfType<TestCharacterComposition>().Which.Marker.Should().Be("initialized-by-test");
            else
                loaded.Build.Composition.Should().BeOfType<CharacterComposition>().Which.Build.Stage.Should().Be("awaiting-choices");
            (await store.GetAsync(created.Id, Guid.NewGuid(), CancellationToken.None)).Should().BeNull();
        }
        (await store.ListAsync(Guid.NewGuid(), 0, CancellationToken.None)).Should().BeEmpty();
        (await store.ListAsync(owner, 0, CancellationToken.None)).Should().HaveCount(modules.Length);
    }

    [TestMethod]
    public async Task FailureAfterRootInsert_RollsBackRootAndCompositionTogether()
    {
        var ruleset = new TestRuleset();
        var binding = new CharacterContentBinding("fixture", "1.0.0", "fixture:character");
        var character = Character.Create(Guid.NewGuid(), "Mira", new(ruleset, binding, ruleset.Initialize(binding)));
        _failure.FailNextInsert = true;
        var create = () => Store(new([ruleset])).CreateAsync(character, CancellationToken.None);
        await create.Should().ThrowAsync<DbUpdateException>();
        _failure.InsertsObserved.Should().BeGreaterThan(0);
        await using var context = new CharactersContext(_options);
        (await context.Set<Character>().CountAsync(testContext.CancellationToken)).Should().Be(0);
        (await context.Set<CharacterBuildRecord>().CountAsync(testContext.CancellationToken)).Should().Be(0);
    }

    [TestMethod]
    [DataRow("""{"Marker":"invalid"}""")]
    [DataRow("""{}""")]
    [DataRow("""{"Marker":"initialized-by-test","Marker":"initialized-by-test"}""")]
    [DataRow("""{"Marker":"initialized-by-test","Unexpected":true}""")]
    public async Task InvalidPersistedComposition_IsNotSilentlyReinitialized(string json)
    {
        var ruleset = new TestRuleset();
        var registry = new CharacterRulesets([ruleset]);
        var binding = new CharacterContentBinding("fixture", "1.0.0", "fixture:character");
        var owner = Guid.NewGuid();
        var character = Character.Create(owner, "Mira", new(ruleset, binding, ruleset.Initialize(binding)));
        await Store(registry).CreateAsync(character, CancellationToken.None);
        await using (var context = new CharactersContext(_options))
        {
            var record = await context.Set<CharacterBuildRecord>().SingleAsync(testContext.CancellationToken);
            record.Composition = json;
            await context.SaveChangesAsync(testContext.CancellationToken);
        }
        var load = () => Store(registry).GetAsync(character.Id, owner, CancellationToken.None);
        await load.Should().ThrowAsync<Exception>();
        await using var unchanged = new CharactersContext(_options);
        (await unchanged.Set<CharacterBuildRecord>().SingleAsync(testContext.CancellationToken)).Composition.Should().Be(json);
    }

    private CharacterBuildStore Store(CharacterRulesets registry)
        => new(new PooledDbContextFactory<CharactersContext>(_options), registry, NullLogger<CharacterBuildStore>.Instance);

    [TestMethod]
    public async Task AcceptedFactsAndPendingReceipt_WhenPersistenceFails_RollBackTogether()
    {
        var ruleset = new Ruleset();
        var registry = new CharacterRulesets([ruleset]);
        var content = new CharacterContentBinding("srd-5.2.1", "0.2.0", "srd521:character:default");
        var owner = Guid.NewGuid();
        var root = Character.Create(owner, "Before", new(ruleset, content, ruleset.Initialize(content)));
        await Store(registry).CreateAsync(root, testContext.CancellationToken);
        root.Rename("Unsaved");
        var edits = new CharacterEditStore(new PooledDbContextFactory<CharactersContext>(_options), registry, NullLogger<CharacterEditStore>.Instance);
        _failure.FailNextInsert = true;
        var save = () => edits.AcceptAsync(root, 0, Guid.NewGuid(), "fingerprint", testContext.CancellationToken);
        await save.Should().ThrowAsync<DbUpdateException>();
        var saved = await edits.ReadAsync(root.Id, owner, testContext.CancellationToken);
        saved!.Character.Name.Should().Be("Before");
        saved.Revision.Should().Be(0);
        saved.Status.Should().Be("ready");
        await using var context = new CharactersContext(_options);
        (await context.Set<CharacterEditOperationRecord>().CountAsync(testContext.CancellationToken)).Should().Be(0);
    }

    [TestMethod]
    public async Task OriginalPinnedComposition_ReopensWithoutAddingMechanicsOrRebinding()
    {
        var ruleset = new Ruleset();
        var registry = new CharacterRulesets([ruleset]);
        var content = new CharacterContentBinding("srd-5.2.1", "0.1.0", "srd521:character:default");
        var owner = Guid.NewGuid();
        var root = Character.Create(owner, "Existing", new(ruleset, content, ruleset.Initialize(content)));
        await Store(registry).CreateAsync(root, testContext.CancellationToken);
        const string original = """{"Build":{"Stage":"awaiting-choices"}}""";
        await using (var context = new CharactersContext(_options))
        {
            var record = await context.Set<CharacterBuildRecord>().SingleAsync(testContext.CancellationToken);
            record.Composition = original;
            await context.SaveChangesAsync(testContext.CancellationToken);
        }
        var restored = await Store(registry).GetAsync(root.Id, owner, testContext.CancellationToken);
        restored!.Build!.Content.Should().Be(content);
        restored.Build.Composition.Should().BeOfType<CharacterComposition>().Which.Facts.Should().BeNull();
        await using var unchanged = new CharactersContext(_options);
        (await unchanged.Set<CharacterBuildRecord>().SingleAsync(testContext.CancellationToken)).Composition.Should().Be(original);
    }

    private sealed class FailingInsertInterceptor : DbCommandInterceptor
    {
        public bool FailNextInsert { get; set; }
        public int InsertsObserved { get; private set; }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(
            DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            if (command.CommandText.StartsWith("INSERT", StringComparison.Ordinal))
            {
                InsertsObserved++;
                if (FailNextInsert)
                {
                    FailNextInsert = false;
                    result.Dispose();
                    throw new InvalidOperationException("Injected failure after an insert.");
                }
            }
            return ValueTask.FromResult(result);
        }
    }
}
