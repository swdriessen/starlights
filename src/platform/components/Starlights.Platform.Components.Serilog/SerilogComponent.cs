using Microsoft.Extensions.Hosting;
using Serilog;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Platform.Components.Serilog;

public class SerilogComponent : IPlatformServicesExtension
{
    public int RegistrationOrder => 100;

    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog(configuration =>
        {
            configuration.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}");
            configuration.WriteTo.OpenTelemetry().MinimumLevel.Information();
        });
    }
}
