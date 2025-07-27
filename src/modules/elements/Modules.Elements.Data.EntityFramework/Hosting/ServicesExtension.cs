using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Data;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Modules.Elements.Data.EntityFramework.Hosting;

internal class ServicesExtension : IPlatformServicesExtension
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IElementsRepository, ElementsRepository>();

        // entity framework services
        builder.Services.AddSingleton<IPersistenceContextFactory, PersistenceContextFactory>();
        builder.Services.AddDbContextFactory<ElementsContext>(options =>
        {
            // TODO: Configure the database connection string from configuration
            options.UseInMemoryDatabase("mem");

            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });
    }
}
