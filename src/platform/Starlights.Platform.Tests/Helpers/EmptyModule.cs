using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Platform.Tests.Helpers;

internal sealed class EmptyModule : IPlatformModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<EmptyService>();
    }

    internal class EmptyService
    {
        // This class is just a placeholder to ensure that the module can be resolved and its services can be configured.
    }
}
