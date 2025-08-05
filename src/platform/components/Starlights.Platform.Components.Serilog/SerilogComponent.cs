using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
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
            if (builder.Environment.IsProduction())
            {
                // production logging
                configuration.MinimumLevel.Information();
            }
            else
            {
                configuration.MinimumLevel.Debug();

                configuration.Filter.ByExcluding(e =>
                {
                    // filter out debug logs from EF / MS
                    if (e.Level == LogEventLevel.Debug || e.Level == LogEventLevel.Information)
                    {
                        if (e.Properties.TryGetValue("SourceContext", out var sourceContext))
                        {
                            return sourceContext.ToString().Contains("Microsoft.EntityFrameworkCore", StringComparison.OrdinalIgnoreCase) ||
                            sourceContext.ToString().Contains("Microsoft.AspNetCore", StringComparison.OrdinalIgnoreCase) ||
                            sourceContext.ToString().Contains("Microsoft.Extensions", StringComparison.OrdinalIgnoreCase);
                        }
                    }

                    return false;
                });
            }

            configuration.WriteTo.OpenTelemetry();
            configuration.WriteTo.Console(outputTemplate: ConsoleOutputTemplate);
        });
    }
}
