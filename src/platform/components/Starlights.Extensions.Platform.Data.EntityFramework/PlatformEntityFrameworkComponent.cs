using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Data;
using Starlights.Platform.Hosting;

namespace Starlights.Platform.Components.Data.EntityFramework;

/// <summary>
/// The component for the Starlights Platform that configures the Entity Framework services.
/// </summary>
public class PlatformEntityFrameworkComponent : IPlatformServiceComponent
{
    public int RegistrationOrder => 1000;

    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPersistence, Persistence>();
    }
}