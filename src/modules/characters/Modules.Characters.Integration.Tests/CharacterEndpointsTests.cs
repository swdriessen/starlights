using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AwesomeAssertions;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Starlights.Modules.Characters.Data.EntityFramework;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Data.EntityFramework.Building;
using Starlights.Modules.Characters.Domain.Building;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Endpoints.Building;
using Starlights.Modules.Characters.Endpoints.Characters.CreateCharacter;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacterDetails;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;
using Starlights.Modules.Identity.Accounts;
using Starlights.Modules.Identity.Features.Antiforgery;
using Starlights.Modules.Identity.Features.SignIn;

namespace Starlights.Modules.Characters.Integration.Tests;

[TestClass]
public sealed class CharacterEndpointsTests(TestContext testContext)
{
    private const string Password = "Character-fixture-only-42!";
    private string _directory = null!;
    private CharacterApplicationFactory _factory = null!;

    [TestInitialize]
    public async Task Initialize()
    {
        _directory = Path.Combine(Directory.GetCurrentDirectory(), ".agent-work", $"CharacterEndpoints.{Guid.NewGuid():N}");
        Directory.CreateDirectory(_directory);
        _factory = new(_directory);
        await _factory.InitializeAsync(testContext.CancellationToken);
        using var scope = _factory.Services.CreateScope();
        var accounts = scope.ServiceProvider.GetRequiredService<IAccountAdministration>();
        await accounts.CreateAsync("owner", Password, testContext.CancellationToken);
        await accounts.CreateAsync("other", Password, testContext.CancellationToken);
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await _factory.DisposeAsync();
        Directory.Delete(_directory, recursive: true);
    }

    [TestMethod]
    public async Task CreateListReopenAndRestart_PreserveIdentityAndExactContent_WithRealCookies()
    {
        using var browser = await SignIn("owner");
        var first = await Create(browser, new() { RulesetId = "dnd5e5", Name = "Mira" });
        var second = await Create(browser, new() { RulesetId = "DND5E5", Name = "Mira" });
        first.Should().NotBe(second);
        var details = await browser.GetFromJsonAsync<GetCharacterDetailsResponse>($"/api/characters/{first}", testContext.CancellationToken);
        details!.Character.Name.Should().Be("Mira");
        details.Character.RulesetId.Should().Be("DND5E5");
        details.Character.Status.Should().Be("incomplete");
        details.Character.Level.Should().BeNull();
        details.Character.Content.Should().Be(new CharacterContentResponse("srd-5.2.1", "0.2.0", "srd521:character:default"));
        var list = await browser.GetFromJsonAsync<GetCharactersResponse>("/api/characters", testContext.CancellationToken);
        list!.Characters.Select(character => character.CharacterId).Should().BeEquivalentTo([first, second]);

        await _factory.DisposeAsync();
        _factory = new(_directory);
        await _factory.InitializeAsync(testContext.CancellationToken);
        using var reopened = await SignIn("owner");
        var restored = await reopened.GetFromJsonAsync<GetCharacterDetailsResponse>($"/api/characters/{first}", testContext.CancellationToken);
        restored!.Character.Should().BeEquivalentTo(details.Character);
    }

    [TestMethod]
    [DataRow(null, "The Nameless One")]
    [DataRow("", "The Nameless One")]
    [DataRow("  ", "The Nameless One")]
    [DataRow("boundary", "boundary")]
    public async Task Names_HavePlaceholderAndAcceptTheMaximum(string? name, string expected)
    {
        using var browser = await SignIn("owner");
        if (name == "boundary") name = expected = new string('x', 100);
        var id = await Create(browser, new() { Name = name, RulesetId = "DND5E5" });
        var details = await browser.GetFromJsonAsync<GetCharacterDetailsResponse>($"/api/characters/{id}", testContext.CancellationToken);
        details!.Character.Name.Should().Be(expected);
    }

    [TestMethod]
    public async Task InvalidSelectionsAndOverlongNames_DoNotCreateCharacters()
    {
        using var browser = await SignIn("owner");
        CreateCharacterRequest[] invalid =
        [
            new() { Name = new string('x', 101), RulesetId = "DND5E5" },
            new() { Name = "Mira", RulesetId = "TEST" },
            new() { Name = "Mira", RulesetId = "unknown" },
            new() { Name = "Mira" },
            new() { Name = "Mira", RulesetId = "DND5E5", PackVersion = "latest" },
            new() { Name = "Mira", RulesetId = "DND5E5", PortraitUrl = "https://example.invalid/portrait" }
        ];
        foreach (var request in invalid)
        {
            using var response = await Write(browser, "/api/characters", request);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        var list = await browser.GetFromJsonAsync<GetCharactersResponse>("/api/characters", testContext.CancellationToken);
        list!.Characters.Should().BeEmpty();
        _factory.Services.GetRequiredService<CharacterRulesets>().Contains("TEST").Should().BeFalse();
    }

    [TestMethod]
    public async Task OwnershipAuthenticationAntiforgeryAndLegacyGuards_AreEnforced()
    {
        using var anonymous = _factory.Browser();
        using var unauthorized = await anonymous.GetAsync("/api/characters", testContext.CancellationToken);
        unauthorized.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        unauthorized.Headers.Location.Should().BeNull();
        using var owner = await SignIn("owner");
        using var missingToken = await owner.PostAsJsonAsync("/api/characters",
            new CreateCharacterRequest { Name = "Mira", RulesetId = "DND5E5" }, testContext.CancellationToken);
        missingToken.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var id = await Create(owner, new() { Name = "Mira", RulesetId = "DND5E5" });
        using var other = await SignIn("other");
        var list = await other.GetFromJsonAsync<GetCharactersResponse>("/api/characters", testContext.CancellationToken);
        list!.Characters.Should().BeEmpty();
        using var hidden = await other.GetAsync($"/api/characters/{id}", testContext.CancellationToken);
        hidden.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var unknown = await owner.GetAsync($"/api/characters/{Guid.NewGuid()}", testContext.CancellationToken);
        unknown.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var legacyDelete = await Write(owner, $"/api/characters/{id}", method: HttpMethod.Delete);
        legacyDelete.StatusCode.Should().Be(HttpStatusCode.Conflict);
        using var legacyRead = await owner.GetAsync($"/api/characters/{id}/classes", testContext.CancellationToken);
        legacyRead.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [TestMethod]
    [DataRow("missing-state")]
    [DataRow("missing-content")]
    [DataRow("missing-ruleset")]
    public async Task Reopen_UnavailableSavedStateFailsWithoutRepair(string failure)
    {
        using var browser = await SignIn("owner");
        var id = await Create(browser, new() { RulesetId = "DND5E5", Name = "Private Character" });
        var factory = _factory.Services.GetRequiredService<IDbContextFactory<CharactersContext>>();
        await using (var context = await factory.CreateDbContextAsync(testContext.CancellationToken))
        {
            var state = await context.Set<CharacterBuildRecord>().SingleAsync(testContext.CancellationToken);
            if (failure == "missing-state") context.Remove(state);
            else if (failure == "missing-content") state.PackVersion = "9.9.9";
            else
            {
                var root = await context.Set<Character>().SingleAsync(testContext.CancellationToken);
                context.Entry(root).Property(character => character.RulesetId).CurrentValue = "MISSING";
            }
            await context.SaveChangesAsync(testContext.CancellationToken);
        }
        using var response = await browser.GetAsync($"/api/characters/{id}", testContext.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        (await response.Content.ReadAsStringAsync(testContext.CancellationToken)).Should().NotContain("Private Character");
        await using var unchanged = await factory.CreateDbContextAsync(testContext.CancellationToken);
        var persisted = await unchanged.Set<Character>().SingleAsync(testContext.CancellationToken);
        persisted.Name.Should().Be("Private Character");
        persisted.RulesetId.Should().Be(failure == "missing-ruleset" ? "MISSING" : "DND5E5");
        var stored = await unchanged.Set<CharacterBuildRecord>().SingleOrDefaultAsync(testContext.CancellationToken);
        if (failure == "missing-state") stored.Should().BeNull();
        else stored!.PackVersion.Should().Be(failure == "missing-content" ? "9.9.9" : "0.2.0");
    }

    [TestMethod]
    public async Task Collection_PagesWithoutDuplicates_AndRejectsInvalidOffsets()
    {
        using var browser = await SignIn("owner");
        var created = new List<Guid>();
        for (var index = 0; index < 51; index++)
            created.Add(await Create(browser, new() { RulesetId = "DND5E5", Name = "Mira" }));
        var first = await browser.GetFromJsonAsync<GetCharactersResponse>("/api/characters", testContext.CancellationToken);
        first!.Characters.Should().HaveCount(50);
        first.NextOffset.Should().Be(50);
        var second = await browser.GetFromJsonAsync<GetCharactersResponse>("/api/characters?offset=50", testContext.CancellationToken);
        second!.Characters.Should().ContainSingle();
        second.NextOffset.Should().BeNull();
        first.Characters.Concat(second.Characters).Select(character => character.CharacterId)
            .Should().OnlyHaveUniqueItems().And.BeEquivalentTo(created);
        var repeated = await browser.GetFromJsonAsync<GetCharactersResponse>("/api/characters", testContext.CancellationToken);
        repeated!.Characters.Select(character => character.CharacterId)
            .Should().Equal(first.Characters.Select(character => character.CharacterId));
        foreach (var offset in new[] { "-1", "2147483647", "invalid" })
        {
            using var invalid = await browser.GetAsync($"/api/characters?offset={offset}", testContext.CancellationToken);
            invalid.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    [TestMethod]
    public async Task Endpoints_AreRegisteredOnce_AndOptionsComeFromBundledContent()
    {
        using var browser = await SignIn("owner");
        var options = await browser.GetFromJsonAsync<RulesetOptionResponse[]>("/api/characters/rulesets", testContext.CancellationToken);
        options.Should().ContainSingle().Which.Content.PackVersion.Should().Be("0.2.0");
        var routes = _factory.Services.GetRequiredService<EndpointDataSource>().Endpoints.OfType<RouteEndpoint>()
            .SelectMany(endpoint => endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods
                .Select(method => $"{method} {endpoint.RoutePattern.RawText}") ?? [])
            .Where(route => route.Contains("/api/characters", StringComparison.Ordinal));
        routes.Should().OnlyHaveUniqueItems();
    }

    [TestMethod]
    public async Task ExplicitTestHost_CreatesAndReopensSyntheticStateThroughTheSameApi()
    {
        await _factory.DisposeAsync();
        _factory = new(_directory, testRuleset: true);
        await _factory.InitializeAsync(testContext.CancellationToken);
        using var browser = await SignIn("owner");
        var id = await Create(browser, new() { RulesetId = "test", Name = "Synthetic" });
        var details = await browser.GetFromJsonAsync<GetCharacterDetailsResponse>($"/api/characters/{id}", testContext.CancellationToken);
        details!.Character.RulesetId.Should().Be("TEST");
        details.Character.Content.Should().Be(new CharacterContentResponse("test-initialization", "1.0.0", "test:character:default"));
    }

    [TestMethod]
    public async Task OpenApi_ContainsModernAndRetainedCharacterContracts()
    {
        using var browser = _factory.Browser();
        var json = await browser.GetStringAsync("/openapi/v1.json", testContext.CancellationToken);
        using var document = JsonDocument.Parse(json);
        var paths = document.RootElement.GetProperty("paths");
        paths.GetProperty("/api/characters").GetProperty("post").GetProperty("responses").GetProperty("201")
            .ValueKind.Should().Be(JsonValueKind.Object);
        paths.TryGetProperty("/api/characters/rulesets", out _).Should().BeTrue();
        paths.TryGetProperty("/api/characters/{characterId}/classes", out _).Should().BeTrue();
        if (Environment.GetEnvironmentVariable("STARLIGHTS_OPENAPI_OUTPUT") is { Length: > 0 } output)
            await File.WriteAllTextAsync(output, json, testContext.CancellationToken);
    }

    private async Task<HttpClient> SignIn(string username)
    {
        var browser = _factory.Browser();
        using var response = await Write(browser, "/api/identity/sign-in", new SignInRequest { Username = username, Password = Password });
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        return browser;
    }

    [TestMethod]
    public async Task ReferenceJourney_ClaimsIndependentlyAndPersistsAnIndependentlyVerifiedSheet()
    {
        using var browser = await SignIn("owner");
        var id = await Create(browser, new() { RulesetId = "DND5E5", Name = "Reference" });
        var state = await Build(browser, id);
        state.BuildComplete.Should().BeFalse();
        state.SheetReady.Should().BeTrue();
        state.Abilities.Select(ability => ability.Input).Should().Equal(15, 14, 13, 12, 10, 8);
        state.Abilities.Should().OnlyContain(ability => !ability.IsFallback && ability.Issue == null);
        async Task Choose(string suffix, params string[] answers)
        {
            var selection = state.Selections.Single(selection => selection.Id.EndsWith(suffix, StringComparison.Ordinal));
            state = await Action(browser, id, "choices", new RecordCharacterChoiceRequest(state.Revision, Guid.NewGuid(), selection.Id, answers));
        }
        await Choose("/class", "srd521:class:fighter");
        await Choose("/species", "srd521:species:human");
        await Choose("/background", "srd521:background:soldier");
        await Choose("/size", "srd521:size:medium");
        state = await Action(browser, id, "ability-scores", new AssignCharacterAbilitiesRequest(state.Revision, Guid.NewGuid(),
            new() { ["strength"] = 15, ["dexterity"] = 14, ["constitution"] = 13, ["intelligence"] = 8, ["wisdom"] = 10, ["charisma"] = 12 }));
        await Choose("/ability-increases", "srd521:soldier-increases:str-con");
        await Choose("/skills", "srd521:skill:insight", "srd521:skill:survival");
        await Choose("/skillful", "srd521:skill:perception");
        await Choose("/gaming-set", "srd521:gaming-set:dice");
        await Choose("/versatile", "srd521:origin-feat:alert");
        await Choose("/fighting-style", "srd521:fighting-style:defense");
        await Choose("/masteries", "srd521:mastery:greatsword", "srd521:mastery:flail", "srd521:mastery:javelin");
        await Choose("/languages", "srd521:language:dwarvish", "srd521:language:elvish");
        var packages = state.Selections.Where(selection => selection.Id.EndsWith("/equipment", StringComparison.Ordinal)).ToArray();
        foreach (var selection in packages)
        {
            var option = selection.Options.Single(option => option.Id.EndsWith(":a", StringComparison.Ordinal));
            state = await Action(browser, id, "choices", new RecordCharacterChoiceRequest(state.Revision, Guid.NewGuid(), selection.Id, [option.Id]));
        }
        state.BuildComplete.Should().BeTrue();
        state.OwnedItems.Should().BeEmpty();
        state.Sheet!.Gold.Should().Be(0);
        foreach (var package in packages)
            state = await Action(browser, id, "equipment-claims", new ClaimCharacterEquipmentRequest(state.Revision, Guid.NewGuid(), package.Id));
        var armor = state.OwnedItems.Single(item => item.CanWear);
        var greatsword = state.OwnedItems.Single(item => item.Name == "Greatsword");
        state = await Action(browser, id, "equipment", new EquipCharacterRequest(state.Revision, Guid.NewGuid(), armor.Id, greatsword.Id));
        state.Abilities.Select(ability => ability.Score).Should().Equal(17, 14, 14, 8, 10, 12);
        var numbers = state.Sheet!.Statistics.ToDictionary(statistic => statistic.Id, statistic => statistic.Value);
        // Expected values are independently derived from the official SRD, not from this evaluator.
        foreach (var expected in new Dictionary<string, int>
        {
            ["proficiency-bonus"] = 2, ["hit-points"] = 12, ["speed"] = 30, ["armor-class"] = 17, ["initiative"] = 4,
            ["strength.save"] = 5, ["constitution.save"] = 4, ["athletics"] = 5, ["intimidation"] = 3, ["insight"] = 2,
            ["survival"] = 2, ["perception"] = 2, ["passive-perception"] = 12, ["second-wind-uses"] = 2
        }) numbers[expected.Key].Should().Be(expected.Value, expected.Key);
        state.Sheet.Statistics.Should().OnlyContain(statistic => statistic.Sources.Count > 0);
        state.Sheet.Attacks.Should().ContainSingle().Which.Should().Be(new CharacterAttackResponse("Greatsword", 5, "2d6+3 slashing", "Graze"));
        state.Sheet!.Gold.Should().Be(18);
        var before = state;
        state = await Action(browser, id, "equipment-claims", new ClaimCharacterEquipmentRequest(state.Revision, Guid.NewGuid(), packages[0].Id));
        state.OwnedItems.Should().BeEquivalentTo(before.OwnedItems);
        state.Sheet!.Gold.Should().Be(18);
        await _factory.DisposeAsync();
        _factory = new(_directory);
        await _factory.InitializeAsync(testContext.CancellationToken);
        using var reopened = await SignIn("owner");
        (await Build(reopened, id)).Should().BeEquivalentTo(state, options => options.Excluding(snapshot => snapshot.AcceptedRevision));
    }

    [TestMethod]
    public async Task ReliableEdits_RejectStaleInvalidAndMismatchedRetries_AndPreserveOriginalAcceptance()
    {
        using var browser = await SignIn("owner");
        var id = await Create(browser, new() { RulesetId = "DND5E5" });
        using var missingRevision = await Write(browser, $"/api/characters/{id}/name", new { OperationId = Guid.NewGuid(), Name = "Missing revision" });
        missingRevision.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var original = new NameCharacterRequest(0, Guid.NewGuid(), "First");
        var first = await Action(browser, id, "name", original);
        first.Revision.Should().Be(1);
        var second = await Action(browser, id, "name", new NameCharacterRequest(1, Guid.NewGuid(), "Second"));
        var replay = await Action(browser, id, "name", original);
        replay.Revision.Should().Be(2);
        replay.AcceptedRevision.Should().Be(1);
        replay.Name.Should().Be("Second");
        using var stale = await Write(browser, $"/api/characters/{id}/name", new NameCharacterRequest(0, Guid.NewGuid(), "Stale"));
        stale.StatusCode.Should().Be(HttpStatusCode.Conflict);
        using var mismatch = await Write(browser, $"/api/characters/{id}/name", original with { Name = "Different payload" });
        mismatch.StatusCode.Should().Be(HttpStatusCode.Conflict);
        foreach (var input in new[]
        {
            new Dictionary<string,int> { ["strength"] = 15, ["dexterity"] = 15 },
            new Dictionary<string,int> { ["unknown"] = 14 },
            new Dictionary<string,int> { ["strength"] = 99 }
        })
        {
            using var invalid = await Write(browser, $"/api/characters/{id}/ability-scores", new AssignCharacterAbilitiesRequest(2, Guid.NewGuid(), input));
            invalid.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        (await Build(browser, id)).Should().BeEquivalentTo(second, options => options.Excluding(snapshot => snapshot.AcceptedRevision));
        var partial = await Action(browser, id, "ability-scores", new AssignCharacterAbilitiesRequest(2, Guid.NewGuid(), new() { ["strength"] = 15 }));
        partial.BuildComplete.Should().BeFalse();
        partial.Abilities.Single(ability => ability.Id == "strength").Input.Should().Be(15);
        using var other = await SignIn("other");
        using var hidden = await other.GetAsync($"/api/characters/{id}/build", testContext.CancellationToken);
        hidden.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var hiddenWrite = await Write(other, $"/api/characters/{id}/name", new NameCharacterRequest(3, Guid.NewGuid(), "Stolen"));
        hiddenWrite.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var claims = _factory.Services.GetRequiredService<IDbContextFactory<CharactersContext>>();
        await using var context = await claims.CreateDbContextAsync(testContext.CancellationToken);
        (await context.Set<CharacterEditOperationRecord>().CountAsync(testContext.CancellationToken)).Should().Be(3);
    }

    [TestMethod]
    public async Task ConcurrentEdits_CommitExactlyOneRevisionAndReceipt()
    {
        using var browser = await SignIn("owner");
        var id = await Create(browser, new() { RulesetId = "DND5E5" });
        var results = await Task.WhenAll(Write(browser, $"/api/characters/{id}/name", new NameCharacterRequest(0, Guid.NewGuid(), "A")),
            Write(browser, $"/api/characters/{id}/name", new NameCharacterRequest(0, Guid.NewGuid(), "B")));
        try { results.Select(result => result.StatusCode).Should().BeEquivalentTo([HttpStatusCode.OK, HttpStatusCode.Conflict]); }
        finally { foreach (var result in results) result.Dispose(); }
        var state = await Build(browser, id);
        state.Revision.Should().Be(1);
        await using var context = await _factory.Services.GetRequiredService<IDbContextFactory<CharactersContext>>().CreateDbContextAsync(testContext.CancellationToken);
        (await context.Set<CharacterEditOperationRecord>().CountAsync(testContext.CancellationToken)).Should().Be(1);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task AcceptedWork_AfterRequestCancellation_PersistsOutcomeAndNeverShowsAnOldSheet(bool fail)
    {
        await _factory.DisposeAsync();
        var controlled = new ControlledCharacterCalculation(fail);
        _factory = new(_directory, configure: services => services.AddSingleton<ICharacterSheetCalculation>(controlled));
        using var browser = await SignIn("owner");
        var id = await Create(browser, new() { RulesetId = "DND5E5" });
        using var cancellation = new CancellationTokenSource();
        var token = await browser.GetFromJsonAsync<AntiforgeryResponse>("/api/identity/antiforgery", testContext.CancellationToken);
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/characters/{id}/name")
        { Content = JsonContent.Create(new NameCharacterRequest(0, Guid.NewGuid(), "Accepted")) };
        request.Headers.Add(token!.HeaderName, token.RequestToken);
        var sending = browser.SendAsync(request, cancellation.Token);
        await controlled.Started.Task.WaitAsync(TimeSpan.FromSeconds(20), testContext.CancellationToken);
        var pending = await Build(browser, id);
        pending.Status.Should().Be("pending");
        pending.Name.Should().Be("Accepted");
        pending.Sheet.Should().BeNull();
        pending.SheetReady.Should().BeFalse();
        cancellation.Cancel();
        controlled.Release.TrySetResult();
        try { using var response = await sending; } catch (OperationCanceledException) { }
        CharacterBuildResponse result = pending;
        for (var attempt = 0; attempt < 100 && result.Status == "pending"; attempt++)
        {
            await Task.Delay(20, testContext.CancellationToken);
            result = await Build(browser, id);
        }
        result.Status.Should().Be(fail ? "failed" : "ready");
        result.Revision.Should().Be(1);
        result.Name.Should().Be("Accepted");
        if (fail)
        {
            result.Sheet.Should().BeNull();
            result.Failure.Should().NotContain("Private internal failure");
        }
    }

    [TestMethod]
    public async Task Restart_PreservesAcceptedFactsAndExplicitlyFailsInterruptedOperations()
    {
        using var browser = await SignIn("owner");
        var id = await Create(browser, new() { RulesetId = "DND5E5" });
        await using (var context = await _factory.Services.GetRequiredService<IDbContextFactory<CharactersContext>>().CreateDbContextAsync(testContext.CancellationToken))
        {
            var state = await context.Set<CharacterBuildRecord>().Include(record => record.Character).SingleAsync(testContext.CancellationToken);
            state.Revision = 1;
            state.Status = "pending";
            state.OperationId = Guid.NewGuid();
            context.Entry(state.Character).Property(root => root.Name).CurrentValue = "Durably accepted";
            context.Add(new CharacterEditOperationRecord { CharacterId = state.CharacterId, OperationId = state.OperationId.Value, Revision = 1, Fingerprint = "restart" });
            await context.SaveChangesAsync(testContext.CancellationToken);
        }
        await _factory.DisposeAsync();
        _factory = new(_directory);
        using var reopened = await SignIn("owner");
        var restored = await Build(reopened, id);
        restored.Name.Should().Be("Durably accepted");
        restored.Revision.Should().Be(1);
        restored.Status.Should().Be("failed");
        restored.Failure.Should().Contain("restart");
        restored.Sheet.Should().BeNull();
    }

    private async Task<CharacterBuildResponse> Build(HttpClient browser, Guid id)
        => (await browser.GetFromJsonAsync<CharacterBuildResponse>($"/api/characters/{id}/build", testContext.CancellationToken))!;

    [TestMethod]
    public async Task AbilityGeneration_PersistsDiceAcrossAssignmentsRetriesMethodSwitchesAndRestart()
    {
        using var browser = await SignIn("owner");
        var id = await Create(browser, new() { RulesetId = "DND5E5" });
        var initial = await Build(browser, id);
        initial.AbilityGeneration.Method.Should().Be("standard-array");
        initial.AbilityGeneration.Rolls.Should().BeEmpty();
        initial.Abilities.Select(ability => ability.Input).Should().Equal(15, 14, 13, 12, 10, 8);
        var rollRequest = new ChangeCharacterAbilityGenerationRequest(0, Guid.NewGuid(), "application-rolls", ConfirmReplacement: true);
        var rolled = await Action(browser, id, "ability-generation", rollRequest);
        rolled.AbilityGeneration.Rolls.Should().HaveCount(6);
        foreach (var roll in rolled.AbilityGeneration.Rolls)
        {
            roll.Dice.Should().HaveCount(4).And.OnlyContain(die => die >= 1 && die <= 6);
            roll.Total.Should().Be(roll.Dice.OrderDescending().Take(3).Sum());
        }
        rolled.Abilities.Select(ability => ability.Input).Should().Equal(rolled.AbilityGeneration.Rolls.Select(roll => (int?)roll.Total));
        rolled.Abilities.Should().OnlyContain(ability => !ability.IsFallback && ability.Issue == null);
        var inputs = rolled.Abilities.Select((ability, index) => (ability.Id, rolled.AbilityGeneration.Rolls[5 - index].Total))
            .ToDictionary(pair => pair.Id, pair => pair.Total);
        var assigned = await Action(browser, id, "ability-scores", new AssignCharacterAbilitiesRequest(1, Guid.NewGuid(), inputs));
        assigned.Abilities.Select(ability => ability.Score).Should().Equal(inputs.Values.Select(value => (int?)value));
        assigned.Abilities.Select(ability => ability.Modifier).Should().Equal(inputs.Values.Select(value => (int?)Math.Floor((value - 10) / 2d)));
        var replay = await Action(browser, id, "ability-generation", rollRequest);
        replay.AcceptedRevision.Should().Be(1);
        replay.Revision.Should().Be(2);
        replay.AbilityGeneration.Rolls.Should().BeEquivalentTo(rolled.AbilityGeneration.Rolls);
        using var unconfirmed = await Write(browser, $"/api/characters/{id}/ability-generation",
            new ChangeCharacterAbilityGenerationRequest(2, Guid.NewGuid(), "point-buy"));
        unconfirmed.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await Build(browser, id)).Revision.Should().Be(2);
        var switched = await Action(browser, id, "ability-generation", new ChangeCharacterAbilityGenerationRequest(2, Guid.NewGuid(), "point-buy", ConfirmReplacement: true));
        switched.Abilities.Select(ability => ability.Input).Should().Equal(8, 8, 8, 8, 8, 8);
        switched.AbilityGeneration.PointsSpent.Should().Be(0);
        switched.AbilityGeneration.PointsRemaining.Should().Be(27);
        switched.AbilityGeneration.Rolls.Should().BeEquivalentTo(rolled.AbilityGeneration.Rolls);
        var back = await Action(browser, id, "ability-generation", new ChangeCharacterAbilityGenerationRequest(3, Guid.NewGuid(), "application-rolls", ConfirmReplacement: true));
        back.AbilityGeneration.Rolls.Should().BeEquivalentTo(rolled.AbilityGeneration.Rolls);
        back.Abilities.Select(ability => ability.Input).Should().Equal(rolled.AbilityGeneration.Rolls.Select(roll => (int?)roll.Total));
        using var rerollWithoutConfirmation = await Write(browser, $"/api/characters/{id}/ability-generation",
            new ChangeCharacterAbilityGenerationRequest(4, Guid.NewGuid(), "application-rolls", Reroll: true));
        rerollWithoutConfirmation.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var rerollRequest = new ChangeCharacterAbilityGenerationRequest(4, Guid.NewGuid(), "application-rolls", true, true);
        var rerolled = await Action(browser, id, "ability-generation", rerollRequest);
        rerolled.Abilities.Select(ability => ability.Input).Should().Equal(rerolled.AbilityGeneration.Rolls.Select(roll => (int?)roll.Total));
        var rerollReplay = await Action(browser, id, "ability-generation", rerollRequest);
        rerollReplay.Revision.Should().Be(5);
        rerollReplay.AbilityGeneration.Should().BeEquivalentTo(rerolled.AbilityGeneration);
        await Action(browser, id, "name", new NameCharacterRequest(5, Guid.NewGuid(), "Unrelated edit"));
        await _factory.DisposeAsync();
        _factory = new(_directory);
        await _factory.InitializeAsync(testContext.CancellationToken);
        using var reopened = await SignIn("owner");
        var restored = await Build(reopened, id);
        restored.Revision.Should().Be(6);
        restored.AbilityGeneration.Should().BeEquivalentTo(rerolled.AbilityGeneration);
        restored.Abilities.Should().BeEquivalentTo(rerolled.Abilities);
        restored.Abilities.Should().OnlyContain(ability => !ability.IsFallback && ability.Issue == null);
        var stale = await Write(reopened, $"/api/characters/{id}/ability-generation",
            new ChangeCharacterAbilityGenerationRequest(4, Guid.NewGuid(), "physical-dice", ConfirmReplacement: true));
        using (stale) stale.StatusCode.Should().Be(HttpStatusCode.Conflict);
        using var other = await SignIn("other");
        using var hidden = await Write(other, $"/api/characters/{id}/ability-generation",
            new ChangeCharacterAbilityGenerationRequest(6, Guid.NewGuid(), "physical-dice"));
        hidden.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var noCsrf = await reopened.PostAsJsonAsync($"/api/characters/{id}/ability-generation",
            new ChangeCharacterAbilityGenerationRequest(6, Guid.NewGuid(), "physical-dice"), testContext.CancellationToken);
        noCsrf.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await Build(reopened, id)).AbilityGeneration.Should().BeEquivalentTo(rerolled.AbilityGeneration);
    }

    [TestMethod]
    public async Task AbilityGeneration_PointBuyAndPhysicalDiceValidateBoundariesWithoutChangingRejectedFacts()
    {
        using var browser = await SignIn("owner");
        var id = await Create(browser, new() { RulesetId = "DND5E5" });
        var state = await Action(browser, id, "ability-generation", new ChangeCharacterAbilityGenerationRequest(0, Guid.NewGuid(), "point-buy", ConfirmReplacement: true));
        state.Abilities.Select(ability => ability.Input).Should().Equal(8, 8, 8, 8, 8, 8);
        state.AbilityGeneration.PointsRemaining.Should().Be(27);
        state = await Action(browser, id, "ability-scores", new AssignCharacterAbilitiesRequest(1, Guid.NewGuid(),
            new() { ["strength"] = 15, ["dexterity"] = 15, ["constitution"] = 15, ["intelligence"] = 8, ["wisdom"] = 8, ["charisma"] = 8 }));
        state.AbilityGeneration.PointsSpent.Should().Be(27);
        state.AbilityGeneration.PointsRemaining.Should().Be(0);
        state.Abilities.Select(ability => ability.Modifier).Should().Equal(2, 2, 2, -1, -1, -1);
        foreach (var inputs in new[] {
            new Dictionary<string, int> { ["strength"] = 7 },
            new Dictionary<string, int> { ["strength"] = 16 },
            new Dictionary<string, int> { ["strength"] = 15, ["dexterity"] = 15, ["constitution"] = 15, ["intelligence"] = 9 }
        })
        {
            using var invalid = await Write(browser, $"/api/characters/{id}/ability-scores", new AssignCharacterAbilitiesRequest(2, Guid.NewGuid(), inputs));
            invalid.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            (await Build(browser, id)).Should().BeEquivalentTo(state, options => options.Excluding(snapshot => snapshot.AcceptedRevision));
        }
        state = await Action(browser, id, "ability-generation", new ChangeCharacterAbilityGenerationRequest(2, Guid.NewGuid(), "physical-dice", ConfirmReplacement: true));
        state.Abilities.Should().OnlyContain(ability => ability.Input == null && ability.IsFallback);
        state = await Action(browser, id, "ability-scores", new AssignCharacterAbilitiesRequest(3, Guid.NewGuid(),
            new() { ["strength"] = 3, ["dexterity"] = 18, ["constitution"] = 18, ["intelligence"] = 3, ["wisdom"] = 10, ["charisma"] = 12 }));
        state.Abilities.Select(ability => ability.Score).Should().Equal(3, 18, 18, 3, 10, 12);
        state.Abilities.Select(ability => ability.Modifier).Should().Equal(-4, 4, 4, -4, 0, 1);
        state.AbilityGeneration.Rolls.Should().BeEmpty();
        foreach (var value in new[] { 0, 2, 19 })
        {
            using var invalid = await Write(browser, $"/api/characters/{id}/ability-scores",
                new AssignCharacterAbilitiesRequest(4, Guid.NewGuid(), new() { ["strength"] = value }));
            invalid.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            (await invalid.Content.ReadAsStringAsync(testContext.CancellationToken)).Should().Contain("3 through 18");
        }
        using var fractional = await Write(browser, $"/api/characters/{id}/ability-scores",
            new { ExpectedRevision = 4, OperationId = Guid.NewGuid(), Inputs = new { strength = 3.5 } });
        fractional.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await Build(browser, id)).Should().BeEquivalentTo(state, options => options.Excluding(snapshot => snapshot.AcceptedRevision));
    }

    [TestMethod]
    [Timeout(30000, CooperativeCancellation = true)]
    public async Task SignalR_UsesAuthenticatedOwnerGroupsAndSendsOnlyRevisionInvalidations()
    {
        using var owner = await SignIn("owner");
        using var other = await SignIn("other");
        var ownId = await Create(owner, new() { RulesetId = "DND5E5" });
        var otherId = await Create(other, new() { RulesetId = "DND5E5" });
        async Task<string> Connect(HttpClient browser)
        {
            using var negotiate = await Write(browser, "/api/characters/notifications/negotiate?negotiateVersion=1");
            negotiate.StatusCode.Should().Be(HttpStatusCode.OK);
            using var response = JsonDocument.Parse(await negotiate.Content.ReadAsStringAsync(testContext.CancellationToken));
            var token = response.RootElement.GetProperty("connectionToken").GetString()!;
            var address = "/api/characters/notifications?id=" + Uri.EscapeDataString(token);
            using var start = await browser.GetAsync(address, testContext.CancellationToken);
            start.StatusCode.Should().Be(HttpStatusCode.OK);
            var csrf = await browser.GetFromJsonAsync<AntiforgeryResponse>("/api/identity/antiforgery", testContext.CancellationToken);
            using var handshake = new HttpRequestMessage(HttpMethod.Post, address)
            { Content = new StringContent("{\"protocol\":\"json\",\"version\":1}\u001e", System.Text.Encoding.UTF8, "text/plain") };
            handshake.Headers.Add(csrf!.HeaderName, csrf.RequestToken);
            using var sent = await browser.SendAsync(handshake, testContext.CancellationToken);
            sent.StatusCode.Should().Be(HttpStatusCode.OK);
            var established = await browser.GetStringAsync(address, testContext.CancellationToken);
            established.Should().Contain("{}");
            return address;
        }
        var ownerAddress = await Connect(owner);
        var otherAddress = await Connect(other);
        await Action(owner, ownId, "name", new NameCharacterRequest(0, Guid.NewGuid(), "Never send this private name"));
        var ownEvent = await owner.GetStringAsync(ownerAddress, testContext.CancellationToken);
        ownEvent.Should().Contain("CharacterChanged").And.Contain(ownId.ToString()).And.NotContain("Never send this private name");
        await Action(other, otherId, "name", new NameCharacterRequest(0, Guid.NewGuid(), "Other private name"));
        var otherEvent = await other.GetStringAsync(otherAddress, testContext.CancellationToken);
        otherEvent.Should().Contain(otherId.ToString()).And.NotContain(ownId.ToString()).And.NotContain("Other private name");
        foreach (var message in ownEvent.Split('\u001e', StringSplitOptions.RemoveEmptyEntries))
        {
            using var payload = JsonDocument.Parse(message);
            if (!payload.RootElement.TryGetProperty("arguments", out var arguments)) continue;
            arguments[0].EnumerateObject().Select(property => property.Name).Should().BeEquivalentTo(["characterId", "revision", "status"]);
        }
        using var ownerClose = await Write(owner, ownerAddress, method: HttpMethod.Delete);
        using var otherClose = await Write(other, otherAddress, method: HttpMethod.Delete);
    }

    private async Task<CharacterBuildResponse> Action(HttpClient browser, Guid id, string action, ICharacterEditRequest body)
    {
        using var response = await Write(browser, $"/api/characters/{id}/{action}", body);
        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync(testContext.CancellationToken));
        return (await response.Content.ReadFromJsonAsync<CharacterBuildResponse>(testContext.CancellationToken))!;
    }

    private async Task<Guid> Create(HttpClient browser, CreateCharacterRequest request)
    {
        using var response = await Write(browser, "/api/characters", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<CreateCharacterResponse>(testContext.CancellationToken);
        response.Headers.Location!.OriginalString.Should().Be($"/api/characters/{created!.Id}");
        return created.Id;
    }

    private async Task<HttpResponseMessage> Write(HttpClient browser, string path, object? body = null, HttpMethod? method = null)
    {
        var token = await browser.GetFromJsonAsync<AntiforgeryResponse>("/api/identity/antiforgery", testContext.CancellationToken)
            ?? throw new InvalidOperationException("Missing antiforgery response.");
        using var request = new HttpRequestMessage(method ?? HttpMethod.Post, path);
        request.Headers.Add(token.HeaderName, token.RequestToken);
        if (body is not null) request.Content = JsonContent.Create(body);
        return await browser.SendAsync(request, testContext.CancellationToken);
    }
}
