using Starlights.Modules.Elements.Data.EntityFramework;
using Starlights.Platform.Components.Serilog;
using Starlights.Platform.Hosting;

namespace Modules.Elements.Data.EntityFramework.MigrationService;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.AddServiceDefaults();

        builder.AddStarlightsPlatform(options =>
        {
            options.AdditionalAssemblies.Add(typeof(ElementsContext).Assembly);
            options.AddSerilogComponent();
        });

        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();

        host.Run();
    }
}