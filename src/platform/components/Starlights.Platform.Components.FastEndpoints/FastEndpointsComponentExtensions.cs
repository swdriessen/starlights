using Starlights.Platform.Hosting;

namespace Starlights.Platform.Components.FastEndpoints;

public static class FastEndpointsComponentExtensions
{
    public static void AddFastEndpointsComponent(this PlatformBuilderOptions options) => options.AdditionalAssemblies.Add(typeof(FastEndpointsComponent).Assembly);
}
