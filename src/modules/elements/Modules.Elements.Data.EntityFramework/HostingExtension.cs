using Microsoft.EntityFrameworkCore;
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
            Console.WriteLine("add AddDbContextFactory service");

            // TODO: Configure the database connection string from configuration
            options.UseInMemoryDatabase("mem");

            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });
    }
}
