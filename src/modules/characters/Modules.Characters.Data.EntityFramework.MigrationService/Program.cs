using Starlights.Modules.Characters.Data.EntityFramework;
using Starlights.Platform.Hosting;

namespace Modules.Characters.Data.EntityFramework.MigrationService;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.AddServiceDefaults();
        builder.AddStarlightsPlatform(options => options.AdditionalAssemblies.Add(typeof(CharactersContext).Assembly));
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}