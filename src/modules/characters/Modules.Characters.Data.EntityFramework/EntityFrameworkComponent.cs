using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Components.Data.EntityFramework;
using Starlights.Platform.Hosting;

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

        //builder.Services.AddSingleton<IPersistenceContextFactory, CharactersPersistenceContextFactory>();
        builder.Services.AddSingleton<PersistenceCharactersContextFactory>();
        builder.Services.AddDbContextFactory<CharactersContext>(options =>
        {
            if (builder.Environment.IsIntegration())
            {
                string uniqueIdentifier = Guid.NewGuid().ToString("N");
                Trace.WriteLine($"running inside an integration scenario, using in-memory db with unique name to avoid conflicts [uniqueIdentifier='{uniqueIdentifier}', context='{nameof(CharactersContext)}']");
                options.UseInMemoryDatabase($"in-memory-integration-{uniqueIdentifier}");
                return;
            }

            const string key = "starlights-db"; // 
            var connectionString = builder.Configuration.GetConnectionString(key);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Trace.WriteLine($"no '{key}' connection string configured, using in-memory db");
                options.UseInMemoryDatabase("in-memory");
            }
            else
            {
                Trace.WriteLine($"using the provided '{key}' connection string: {connectionString}");
                options.UseSqlServer(connectionString);
            }

            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });
    }

    public void UseComponent(IHost host)
    {
        host.UseRepositoryWithContext<ICharactersRepository, PersistenceCharactersContextFactory>();
    }
}
