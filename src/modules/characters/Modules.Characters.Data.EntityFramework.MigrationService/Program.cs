using Starlights.Modules.Characters.Data.EntityFramework;
using Starlights.Platform.Hosting;

namespace Modules.Characters.Data.EntityFramework.MigrationService;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.AddServiceDefaults();
        builder.AddStarlightsPlatform(options =>
        {
            // BUG: this starts up the domain event processor too
            // this should not start in the context of the migration service...

            // how to configure a module to ...
            // split off the DomainEventProcessingService into it's own module/project?
            // and not load it in the migration service?

            options.AdditionalAssemblies.Add(typeof(CharactersContext).Assembly);
        });
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}