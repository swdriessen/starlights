using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Starlights.Modules.Identity.Accounts;
using Starlights.Modules.Identity.Data;

namespace Starlights.Modules.Characters.Integration.Tests;

/// <summary>Explicitly opted-in isolated Kestrel fixture for real-cookie browser verification.</summary>
[TestClass]
public sealed class CharacterBrowserFixtureTests(TestContext testContext)
{
    /// <summary>Runs only when all fixture environment variables are supplied; never touches shared SQL or blob storage.</summary>
    [TestMethod]
    public async Task RunIsolatedBrowserFixture()
    {
        var directoryInput = Environment.GetEnvironmentVariable("STARLIGHTS_BROWSER_FIXTURE_DIRECTORY");
        if (string.IsNullOrWhiteSpace(directoryInput)) Assert.Inconclusive("Browser fixture is opt-in.");
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(root.FullName, "global.json")))
            root = root.Parent ?? throw new InvalidOperationException("The repository root is unavailable.");
        var directory = Path.GetFullPath(directoryInput, root.FullName);
        if (!directory.StartsWith(root.FullName + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The fixture directory must be a descendant of the repository working directory.");
        Directory.CreateDirectory(directory);
        File.Delete(Path.Combine(directory, "ready"));
        File.Delete(Path.Combine(directory, "stop"));
        var port = int.Parse(Environment.GetEnvironmentVariable("STARLIGHTS_BROWSER_FIXTURE_PORT") ?? throw new InvalidOperationException("Fixture port is required."));
        var username = Environment.GetEnvironmentVariable("STARLIGHTS_BROWSER_FIXTURE_USERNAME") ?? throw new InvalidOperationException("Fixture username is required.");
        var password = Environment.GetEnvironmentVariable("STARLIGHTS_BROWSER_FIXTURE_PASSWORD") ?? throw new InvalidOperationException("Fixture password is required.");
        await using var factory = new CharacterApplicationFactory(directory, configure: services =>
        {
            // This opt-in loopback fixture uses HTTP; production cookie/antiforgery policy remains unchanged.
            services.PostConfigure<AntiforgeryOptions>(options => options.Cookie.SecurePolicy = CookieSecurePolicy.None);
            services.ConfigureApplicationCookie(options => options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest);
        });
        factory.UseKestrel(port);
        await factory.InitializeAsync(testContext.CancellationToken);
        await InitializeFixtureAccountAsync(factory.Services, username, password, testContext.CancellationToken);
        using var client = new HttpClient { BaseAddress = new($"http://localhost:{port}") };
        var json = await client.GetStringAsync("/openapi/v1.json", testContext.CancellationToken);
        using var antiforgery = await client.GetAsync("/api/identity/antiforgery", testContext.CancellationToken);
        antiforgery.EnsureSuccessStatusCode();
        await File.WriteAllTextAsync(Path.Combine(directory, "openapi.json"), json, testContext.CancellationToken);
        await File.WriteAllTextAsync(Path.Combine(directory, "ready"), $"http://localhost:{port}", testContext.CancellationToken);
        while (!File.Exists(Path.Combine(directory, "stop")))
            await Task.Delay(250, testContext.CancellationToken);
    }

    [TestMethod]
    public async Task FixtureAccountInitialization_ReusesIdentityWithoutChangingPasswordOrSecurityStamp()
    {
        var directory = Path.Combine(AppContext.BaseDirectory, ".agent-work", $"fixture-account-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            await using var factory = new CharacterApplicationFactory(directory);
            await factory.InitializeAsync(testContext.CancellationToken);
            var password = Guid.NewGuid().ToString() + "!aA42";
            var id = await InitializeFixtureAccountAsync(factory.Services, "fixture-account", password, testContext.CancellationToken);
            string? originalHash, originalStamp;
            using (var scope = factory.Services.CreateScope())
            {
                var user = await scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>().FindByNameAsync("fixture-account");
                Assert.IsNotNull(user);
                originalHash = user.PasswordHash;
                originalStamp = user.SecurityStamp;
            }
            var reused = await InitializeFixtureAccountAsync(factory.Services, "fixture-account",
                Guid.NewGuid().ToString() + "!aA42", testContext.CancellationToken);
            Assert.AreEqual(id, reused);
            using (var scope = factory.Services.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await users.FindByNameAsync("fixture-account");
                Assert.IsNotNull(user);
                Assert.AreEqual(originalHash, user.PasswordHash);
                Assert.AreEqual(originalStamp, user.SecurityStamp);
                Assert.IsTrue(await users.CheckPasswordAsync(user, password));
            }
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static async Task<Guid> InitializeFixtureAccountAsync(IServiceProvider services, string username,
        string password, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var scope = services.CreateScope();
        var existing = await scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>().FindByNameAsync(username);
        return existing?.Id ?? await scope.ServiceProvider.GetRequiredService<IAccountAdministration>()
            .CreateAsync(username, password, cancellationToken);
    }
}
