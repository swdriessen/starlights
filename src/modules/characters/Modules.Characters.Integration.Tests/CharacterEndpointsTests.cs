using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AwesomeAssertions;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Starlights.Modules.Characters.Data.EntityFramework;
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
        _directory = Path.Combine(Path.GetTempPath(), $"Starlights.CharacterEndpoints.{Guid.NewGuid():N}");
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
        details.Character.Content.Should().Be(new CharacterContentResponse("srd-5.2.1", "0.1.0", "srd521:character:default"));
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
        else stored!.PackVersion.Should().Be(failure == "missing-content" ? "9.9.9" : "0.1.0");
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
        options.Should().ContainSingle().Which.Content.PackVersion.Should().Be("0.1.0");
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
