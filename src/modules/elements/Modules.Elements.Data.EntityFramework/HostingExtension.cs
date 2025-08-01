using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Data;
using Starlights.Platform.Hosting;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Modules.Elements.Data.EntityFramework;

/// <summary>
/// Hosting extension for the Elements module that configures the Entity Framework services.
/// </summary>
internal class HostingExtension : IPlatformServicesExtension
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IElementsRepository, ElementsRepository>();

        // entity framework services
        builder.Services.AddSingleton<IPersistenceContextFactory, PersistenceContextFactory>();
        builder.Services.AddDbContextFactory<ElementsContext>(options =>
        {
            if (builder.Environment.IsIntegration())
            {
                string uniqueIdentifier = Guid.NewGuid().ToString("N");
                Trace.WriteLine($"running inside an integration scenario, using in-memory db with unique name to avoid conflicts [uniqueIdentifier='{uniqueIdentifier}']");
                options.UseInMemoryDatabase($"in-memory-integration-{uniqueIdentifier}");
                options.EnableSensitiveDataLogging();
                return;
            }

            var connectionString = builder.Configuration.GetConnectionString("starlights");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Trace.WriteLine("no 'DefaultConnection' connection string configured, using in-memory db");
                options.UseInMemoryDatabase("in-memory");
            }
            else
            {
                Trace.WriteLine($"using the provided 'DefaultConnection' connection string: {connectionString}");
                options.UseSqlServer(connectionString);
            }

            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });
    }
}
