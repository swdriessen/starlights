using Microsoft.Extensions.DependencyInjection;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Platform.Hosting;

public sealed class PlatformBuilder : IPlatformBuilder
{
    public PlatformBuilder(IServiceCollection services, PlatformBuilderOptions options)
    {
        Services = services;
        Options = options;
    }

    public IServiceCollection Services { get; }

    public PlatformBuilderOptions Options { get; }

    public void Build()
    {
        this.RegisterPlatformModules();
    }
}
