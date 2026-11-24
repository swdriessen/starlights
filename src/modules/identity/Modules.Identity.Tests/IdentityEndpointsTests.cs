using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Starlights.Modules.Characters.Data.EntityFramework;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.Services;
using Starlights.Modules.Identity.Accounts;
using Starlights.Modules.Identity.Features.Antiforgery;
using Starlights.Modules.Identity.Features.Session;
using Starlights.Modules.Identity.Features.SignIn;

[assembly: DoNotParallelize]

namespace Starlights.Modules.Identity.Tests;

[TestClass]
public sealed class IdentityEndpointsTests(TestContext testContext)
{
    private const string Password = "Only-for-local-tests-42!";
    private string _directory = null!;
    private IdentityApplicationFactory _factory = null!;
    private TestClock _clock = null!;

    [TestInitialize]
    public async Task Initialize()
    {
        _directory = Path.Combine(Path.GetTempPath(), $"Starlights.Identity.Tests.{Guid.NewGuid():N}");
        Directory.CreateDirectory(_directory);
        _clock = new TestClock();
        _factory = new IdentityApplicationFactory(Path.Combine(_directory, "identity.db"), _clock);
        await _factory.InitializeAsync();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await _factory.DisposeAsync();
        Directory.Delete(_directory, recursive: true);
    }

    [TestMethod]
    public async Task ProvisionSignInAndRestart_PreservesIdentityAndSession()
    {
        var id = await CreateAccount("first");
        using var browser = _factory.CreateBrowser();
        using var signIn = await SignIn(browser, "first", Password);
        signIn.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var sessionCookie = SessionCookie(signIn);
        signIn.Headers.GetValues("Set-Cookie")
            .Single(cookie => cookie.StartsWith("Starlights.Session=", StringComparison.Ordinal))
            .Should().NotContain("expires=");
        var session = await browser.GetFromJsonAsync<SessionResponse>("/api/identity/session", testContext.CancellationToken);
        session!.UserId.Should().Be(id);
        session.Username.Should().Be("first");

        await _factory.DisposeAsync();
        _factory = new IdentityApplicationFactory(Path.Combine(_directory, "identity.db"), _clock);
        await _factory.InitializeAsync();
        using var restoredBrowser = CookieBrowser(sessionCookie);
        var restored = await restoredBrowser.GetFromJsonAsync<SessionResponse>("/api/identity/session", testContext.CancellationToken);
        restored.Should().Be(session);
    }

    [TestMethod]
    public async Task ApiRequests_RequireAuthenticationAndAntiforgeryWithoutRedirects()
    {
        using var browser = _factory.CreateBrowser();
        using var anonymous = await browser.GetAsync("/api/characters", testContext.CancellationToken);
        anonymous.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        anonymous.Headers.Location.Should().BeNull();
        await CreateAccount("first");
        using var noToken = await browser.PostAsJsonAsync("/api/identity/sign-in",
            new SignInRequest { Username = "first", Password = Password }, testContext.CancellationToken);
        noToken.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var failedLogin = await SignIn(browser, "first", "Incorrect-password");
        failedLogin.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        using var login = await SignIn(browser, "first", Password);
        login.StatusCode.Should().Be(HttpStatusCode.NoContent);
        using var missingInitializationToken = await browser.PostAsync("/api/elements/initialize", null, testContext.CancellationToken);
        missingInitializationToken.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var oldInitializationGet = await browser.GetAsync("/api/elements/initialize", testContext.CancellationToken);
        oldInitializationGet.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        using var missingSignOutToken = await browser.PostAsync("/api/identity/sign-out", null, testContext.CancellationToken);
        missingSignOutToken.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var mismatchedToken = new HttpRequestMessage(HttpMethod.Post, "/api/identity/sign-out");
        mismatchedToken.Headers.Add("X-CSRF-TOKEN", "invalid");
        using var invalidSignOut = await browser.SendAsync(mismatchedToken, testContext.CancellationToken);
        invalidSignOut.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task SignOut_RevokesOnlyCurrentSession_EvenIfItsCookieIsReplayed()
    {
        await CreateAccount("first");
        using var first = _factory.CreateBrowser();
        using var second = _factory.CreateBrowser();
        using var firstLogin = await SignIn(first, "first", Password);
        using var secondLogin = await SignIn(second, "first", Password);
        var cookie = SessionCookie(firstLogin);
        using var signOut = await Write(first, "/api/identity/sign-out");
        signOut.StatusCode.Should().Be(HttpStatusCode.NoContent);
        using var replay = CookieBrowser(cookie);
        using var rejected = await replay.GetAsync("/api/identity/session", testContext.CancellationToken);
        rejected.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        using var remaining = await second.GetAsync("/api/identity/session", testContext.CancellationToken);
        remaining.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task ResetPassword_RevokesAllSessionsOnNextRequest_AndPreservesOwnershipId()
    {
        var id = await CreateAccount("first");
        using var first = _factory.CreateBrowser();
        using var second = _factory.CreateBrowser();
        using var firstLogin = await SignIn(first, "first", Password);
        using var secondLogin = await SignIn(second, "first", Password);
        using (var scope = _factory.Services.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<IAccountAdministration>()
                .ResetPasswordAsync("first", Password + "new", CancellationToken.None);
        }
        using var firstRejected = await first.GetAsync("/api/identity/session", testContext.CancellationToken);
        using var secondRejected = await second.GetAsync("/api/identity/session", testContext.CancellationToken);
        firstRejected.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        secondRejected.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        using var browser = _factory.CreateBrowser();
        using var oldLogin = await SignIn(browser, "first", Password);
        oldLogin.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        using var newLogin = await SignIn(browser, "first", Password + "new");
        newLogin.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var session = await browser.GetFromJsonAsync<SessionResponse>("/api/identity/session", testContext.CancellationToken);
        session!.UserId.Should().Be(id);
    }

    [TestMethod]
    public async Task DisableAndReenable_DoesNotResurrectOldSession()
    {
        await CreateAccount("first");
        using var browser = _factory.CreateBrowser();
        using var login = await SignIn(browser, "first", Password);
        var cookie = SessionCookie(login);
        using (var scope = _factory.Services.CreateScope())
        {
            var accounts = scope.ServiceProvider.GetRequiredService<IAccountAdministration>();
            await accounts.SetDisabledAsync("first", true, CancellationToken.None);
            using var disabledBrowser = _factory.CreateBrowser();
            using var rejected = await SignIn(disabledBrowser, "first", Password);
            rejected.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            await accounts.SetDisabledAsync("first", false, CancellationToken.None);
        }
        using var replay = CookieBrowser(cookie);
        using var oldSession = await replay.GetAsync("/api/identity/session", testContext.CancellationToken);
        oldSession.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        using var newBrowser = _factory.CreateBrowser();
        using var newSession = await SignIn(newBrowser, "first", Password);
        newSession.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [TestMethod]
    public async Task Session_ExpiresAfterEightHoursWithoutActivity()
    {
        await CreateAccount("first");
        using var browser = _factory.CreateBrowser();
        using var login = await SignIn(browser, "first", Password);
        _clock.Advance(TimeSpan.FromHours(8) + TimeSpan.FromSeconds(1));
        using var response = await browser.GetAsync("/api/identity/session", testContext.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [TestMethod]
    public async Task AccountProvisioning_RejectsDuplicatesAndWeakPasswords()
    {
        await CreateAccount("first");
        using var scope = _factory.Services.CreateScope();
        var accounts = scope.ServiceProvider.GetRequiredService<IAccountAdministration>();
        var duplicate = () => accounts.CreateAsync("FIRST", Password, CancellationToken.None);
        await duplicate.Should().ThrowAsync<InvalidOperationException>();
        var weak = () => accounts.CreateAsync("second", "weak", CancellationToken.None);
        await weak.Should().ThrowAsync<InvalidOperationException>();
    }

    [TestMethod]
    public async Task LocalCommands_RejectPasswordArguments_AndNoRegistrationEndpointExists()
    {
        var code = await AccountCommands.RunAsync(_factory.Services, ["create", "user", "do-not-accept-passwords-here"], testContext.CancellationToken);
        code.Should().Be(1);
        await CreateAccount("first");
        using var browser = _factory.CreateBrowser();
        using var login = await SignIn(browser, "first", Password);
        using var response = await Write(browser, "/api/identity/register");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task RetainedCharacters_ExcludeOtherOwners_AndHideTheirReadAndDeleteRoutes()
    {
        var firstId = await CreateAccount("first");
        var secondId = await CreateAccount("second");
        var own = await SeedCharacter(firstId, "First private character");
        var other = await SeedCharacter(secondId, "Second private character");
        using var first = _factory.CreateBrowser();
        using var second = _factory.CreateBrowser();
        using var firstLogin = await SignIn(first, "first", Password);
        using var secondLogin = await SignIn(second, "second", Password);

        var list = await first.GetFromJsonAsync<JsonElement>("/api/characters", testContext.CancellationToken);
        list.GetProperty("characters").EnumerateArray().Select(character => character.GetProperty("name").GetString())
            .Should().Equal("First private character");
        using var ownDetails = await first.GetAsync($"/api/characters/{own.Id.Value}", testContext.CancellationToken);
        ownDetails.StatusCode.Should().Be(HttpStatusCode.OK);
        using var hiddenDetails = await first.GetAsync($"/api/characters/{other.Id.Value}", testContext.CancellationToken);
        using var missingDetails = await first.GetAsync($"/api/characters/{Guid.NewGuid()}", testContext.CancellationToken);
        hiddenDetails.StatusCode.Should().Be(HttpStatusCode.NotFound);
        missingDetails.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var hiddenDelete = await Write(first, $"/api/characters/{other.Id.Value}", method: HttpMethod.Delete);
        hiddenDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var preserved = await second.GetAsync($"/api/characters/{other.Id.Value}", testContext.CancellationToken);
        preserved.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task UnregisterSelection_RejectsAnotherCharactersParentRegistration_WithoutChangingSelection()
    {
        var firstId = await CreateAccount("first");
        var secondId = await CreateAccount("second");
        var own = await SeedCharacter(firstId, "First character");
        var other = await SeedCharacter(secondId, "Second character");
        var parent = Registration.Create(other.Id, new ElementId(Guid.NewGuid()), "Parent", "Test");
        var child = Registration.Create(other.Id, new ElementId(Guid.NewGuid()), "Selected", "Test");
        child.SetParentRegistration(parent);
        var rule = parent.CreateSelectionRule(new ElementComponentId(Guid.NewGuid()), "Test", "Choice");
        rule.UpdateCurrentSelection(child);
        using (var scope = _factory.Services.CreateScope())
        {
            await using var db = await scope.ServiceProvider.GetRequiredService<IDbContextFactory<CharactersContext>>().CreateDbContextAsync(testContext.CancellationToken);
            db.AddRange(parent, child);
            await db.SaveChangesAsync(testContext.CancellationToken);
        }
        using var browser = _factory.CreateBrowser();
        using var login = await SignIn(browser, "first", Password);

        using var response = await Write(browser,
            $"/api/characters/{own.Id.Value}/builder/selection-rules/{rule.Id.Value}/unregister",
            new { ParentRegistration = parent.Id.Value, ElementId = child.AssociatedElementId.Value });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        using var verification = _factory.Services.CreateScope();
        await using var context = await verification.ServiceProvider.GetRequiredService<IDbContextFactory<CharactersContext>>().CreateDbContextAsync(testContext.CancellationToken);
        var preserved = await context.Set<Registration>().Include(registration => registration.SelectionRules)
            .SingleAsync(registration => registration.Id == parent.Id, testContext.CancellationToken);
        preserved.SelectionRules.Single().SelectionRegistrationId.Should().Be(child.Id);
        (await context.Set<Registration>().AnyAsync(registration => registration.Id == child.Id, testContext.CancellationToken)).Should().BeTrue();
    }

    private async Task<Character> SeedCharacter(Guid owner, string name)
    {
        using var scope = _factory.Services.CreateScope();
        var accessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        var previous = accessor.HttpContext;
        try
        {
            accessor.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, owner.ToString())], "TestSeed"))
            };
            var character = scope.ServiceProvider.GetRequiredService<ICharacterCreationService>().Create(name);
            await using var context = await scope.ServiceProvider.GetRequiredService<IDbContextFactory<CharactersContext>>().CreateDbContextAsync(testContext.CancellationToken);
            context.Add(character);
            await context.SaveChangesAsync(testContext.CancellationToken);
            return character;
        }
        finally
        {
            accessor.HttpContext = previous;
        }
    }

    private async Task<Guid> CreateAccount(string username)
    {
        using var scope = _factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IAccountAdministration>()
            .CreateAsync(username, Password, CancellationToken.None);
    }

    private async Task<HttpResponseMessage> SignIn(HttpClient browser, string username, string password)
        => await Write(browser, "/api/identity/sign-in", new SignInRequest { Username = username, Password = password });

    private async Task<HttpResponseMessage> Write(HttpClient browser, string path, object? credentials = null, HttpMethod? method = null)
    {
        var tokens = await browser.GetFromJsonAsync<AntiforgeryResponse>("/api/identity/antiforgery", testContext.CancellationToken)
            ?? throw new InvalidOperationException("The antiforgery response is missing.");
        using var request = new HttpRequestMessage(method ?? HttpMethod.Post, path);
        request.Headers.Add(tokens.HeaderName, tokens.RequestToken);
        if (credentials is not null)
        {
            request.Content = JsonContent.Create(credentials);
        }
        return await browser.SendAsync(request, testContext.CancellationToken);
    }

    private static string SessionCookie(HttpResponseMessage response)
        => response.Headers.GetValues("Set-Cookie")
            .Single(cookie => cookie.StartsWith("Starlights.Session=", StringComparison.Ordinal))
            .Split(';')[0];

    private HttpClient CookieBrowser(string cookie)
    {
        var browser = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"), HandleCookies = false, AllowAutoRedirect = false
        });
        browser.DefaultRequestHeaders.Add("Cookie", cookie);
        return browser;
    }
}
