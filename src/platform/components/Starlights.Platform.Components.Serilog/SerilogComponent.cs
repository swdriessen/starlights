using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Filters;
using Starlights.Platform.Hosting;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Platform.Components.Serilog;

public class SerilogComponent : IPlatformServicesExtension
{
    public int RegistrationOrder => 100;

    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog(configuration =>
        {
            configuration.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .MinimumLevel.Information();

            configuration.WriteTo.OpenTelemetry()
                .MinimumLevel.Information();

            if (builder.Environment.IsIntegration())
            {
                // minimize logging noise in integration environments for now, TODO: use appsettings.Integration.json for this
                configuration.Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.Database.Command"));
                configuration.Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore"));
            }
        });
    }
}
