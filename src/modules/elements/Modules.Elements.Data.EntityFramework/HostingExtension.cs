using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Data;
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
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.WriteLine("No connection string configured in appsettings.json or environment variables, using in-memory db.");
                options.UseInMemoryDatabase("in-memory");
            }
            else
            {
                Console.WriteLine($"Using connection string: {connectionString}");
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            }

            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });
    }
}
