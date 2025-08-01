using Microsoft.Extensions.Hosting;
using Serilog;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Platform.Components.Serilog;

public class SerilogComponent : IPlatformServicesExtension
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog();
    }
}
