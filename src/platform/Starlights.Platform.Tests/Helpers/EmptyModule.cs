using Microsoft.Extensions.DependencyInjection;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Platform.Tests.Helpers;

internal sealed class EmptyModule : IPlatformModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<EmptyService>();
    }

    internal class EmptyService
    {
        // This class is just a placeholder to ensure that the module can be resolved and its services can be configured.
    }
}
