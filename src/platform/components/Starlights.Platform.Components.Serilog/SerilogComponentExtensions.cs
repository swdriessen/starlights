using Starlights.Platform.Hosting;

namespace Starlights.Platform.Components.Serilog;

public static class SerilogComponentExtensions
{
    public static void AddSerilogComponent(this PlatformBuilderOptions options) => options.AdditionalAssemblies.Add(typeof(SerilogComponent).Assembly);
}