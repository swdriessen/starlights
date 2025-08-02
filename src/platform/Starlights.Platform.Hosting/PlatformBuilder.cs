using Microsoft.Extensions.DependencyInjection;

namespace Starlights.Platform.Hosting;

public sealed class PlatformBuilder : IPlatformBuilder
{
    public PlatformBuilder(IServiceCollection services, PlatformBuilderOptions options)
    {
        Services = services;
        Options = options;
    }

    public Dictionary<string, object> Properties { get; } = [];

    public IServiceCollection Services { get; }

    public PlatformBuilderOptions Options { get; }

    public void Build()
    {
        this.RegisterPlatformModules();
        this.InvokePlatformServiceComponents();
    }
}
