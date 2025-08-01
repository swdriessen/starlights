using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Data;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Platform.Components.Data.EntityFramework;

/// <summary>
/// Hosting extension for the Starlights Platform that configures the Entity Framework services.
/// </summary>
public class HostingExtension : IPlatformServicesExtension
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPersistence, Persistence>();
    }
}