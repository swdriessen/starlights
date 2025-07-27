using Microsoft.Extensions.DependencyInjection;
using Starlights.Platform.Data;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Extensions.Platform.Data.EntityFramework;

public sealed class ExtensionsPlatformDataEntityFrameworkModule : IPlatformModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IPersistence, Persistence>();
    }
}