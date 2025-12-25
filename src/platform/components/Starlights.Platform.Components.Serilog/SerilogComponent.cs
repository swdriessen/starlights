using Microsoft.Extensions.Hosting;
using Serilog;
using Starlights.Platform.Hosting;

namespace Starlights.Platform.Components.Serilog;

/// <summary>
/// The component for the Starlights Platform that configures Serilog logging services.
/// </summary>
public class SerilogComponent : IPlatformServiceComponent
{
    public const string ConsoleOutputTemplate = "[{Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}";
    public int RegistrationOrder => 100;

    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog(configuration =>
        {
            // load configuration from appsettings
            configuration.ReadFrom.Configuration(builder.Configuration);

            configuration.Enrich.FromLogContext();
            configuration.WriteTo.Console(outputTemplate: ConsoleOutputTemplate);

            // allow aspire dashboard to capture logs
            configuration.WriteTo.OpenTelemetry();
        });
    }
}
