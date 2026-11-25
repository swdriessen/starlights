using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Components.Data.EntityFramework;
using Starlights.Platform.Hosting;
using Starlights.Modules.Characters.Data.EntityFramework.Building;
using Starlights.Modules.Characters.Domain.Building;

namespace Starlights.Modules.Characters.Data.EntityFramework;

/// <summary>
/// The platform component for the Characters module that configures the Entity Framework services.
/// </summary>
internal class EntityFrameworkComponent : IPlatformServiceComponent, IPlatformApplicationComponent
{
    public int RegistrationOrder => 1020;

    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICharactersRepository, CharactersRepository>();
        builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
        builder.Services.AddScoped<ICharacterOwnership, CharacterOwnership>();
        builder.Services.AddScoped<ICharacterBuildStore, CharacterBuildStore>();
        builder.Services.TryAddSingleton<CharacterRulesets>();
        builder.Services.AddSingleton<CharacterOwnershipInterceptor>();

        builder.Services.AddSingleton<PersistenceCharactersContextFactory>();
        builder.Services.AddDbContextFactory<CharactersContext>((services, options) =>
        {
            options.AddInterceptors(services.GetRequiredService<CharacterOwnershipInterceptor>());
            const string key = "starlights-db";
            var connectionString = builder.Configuration.GetConnectionString(key);
            if (builder.Environment.IsIntegration() && string.IsNullOrWhiteSpace(connectionString))
            {
                string uniqueIdentifier = Guid.NewGuid().ToString("N");
                Trace.WriteLine($"running inside an integration scenario, using in-memory db with unique name to avoid conflicts [uniqueIdentifier='{uniqueIdentifier}', context='{nameof(CharactersContext)}']");
                options.UseInMemoryDatabase($"in-memory-integration-{uniqueIdentifier}");
                return;
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Characters requires a durable starlights-db connection outside integration tests.");
            }
            else
            {
                options.UseSqlServer(connectionString);
            }

        });
    }

    public void UseComponent(IHost host)
    {
        host.UseRepositoryWithContext<ICharactersRepository, PersistenceCharactersContextFactory>();
        host.UseRepositoryWithContext<IRegistrationRepository, PersistenceCharactersContextFactory>();
    }
}
