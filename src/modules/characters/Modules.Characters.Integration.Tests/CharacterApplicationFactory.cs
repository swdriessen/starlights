using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Starlights.Application;
using Starlights.Modules.Characters.Building;
using Starlights.Modules.Characters.Data.EntityFramework;
using Starlights.Modules.Characters.Domain.Building;
using Starlights.Modules.Identity.Data;
using Starlights.Modules.FileManagement.Data;
using Starlights.RuleSets.TEST;

namespace Starlights.Modules.Characters.Integration.Tests;

internal sealed class CharacterApplicationFactory(string directory, bool testRuleset = false,
    Action<IServiceCollection>? configure = null) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Integration");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<CharactersContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<CharactersContext>>();
            services.AddDbContextFactory<CharactersContext>((provider, options) => options
                .UseSqlite($"Data Source={Path.Combine(directory, "characters.db")};Pooling=False")
                .AddInterceptors(provider.GetRequiredService<CharacterOwnershipInterceptor>()));
            services.RemoveAll<DbContextOptions<IdentityContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<IdentityContext>>();
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlite($"Data Source={Path.Combine(directory, "identity.db")};Pooling=False"));
            services.RemoveAll<DbContextOptions<FileManagementContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<FileManagementContext>>();
            services.AddDbContext<FileManagementContext>(options =>
                options.UseSqlite($"Data Source={Path.Combine(directory, "files.db")};Pooling=False"));
            if (testRuleset)
            {
                services.AddSingleton<ICharacterRuleset, TestRuleset>();
                services.RemoveAll<CharacterContentCatalog>();
                services.AddSingleton(provider =>
                {
                    var rulesets = provider.GetRequiredService<CharacterRulesets>();
                    var catalog = new CharacterContentCatalog();
                    catalog.AddEmbeddedPack(typeof(TestRuleset).Assembly, "Starlights.RuleSets.TEST.Content.test.json", rulesets);
                    return catalog;
                });
            }
            configure?.Invoke(services);
        });
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await using var identity = new IdentityContext(new DbContextOptionsBuilder<IdentityContext>()
            .UseSqlite($"Data Source={Path.Combine(directory, "identity.db")};Pooling=False").Options);
        await identity.Database.EnsureCreatedAsync(cancellationToken);
        await using var context = new CharactersContext(new DbContextOptionsBuilder<CharactersContext>()
            .UseSqlite($"Data Source={Path.Combine(directory, "characters.db")};Pooling=False").Options);
        await context.Database.EnsureCreatedAsync(cancellationToken);
        await using var files = new FileManagementContext(new DbContextOptionsBuilder<FileManagementContext>()
            .UseSqlite($"Data Source={Path.Combine(directory, "files.db")};Pooling=False").Options);
        await files.Database.EnsureCreatedAsync(cancellationToken);
    }

    public HttpClient Browser() => CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false
    });
}
