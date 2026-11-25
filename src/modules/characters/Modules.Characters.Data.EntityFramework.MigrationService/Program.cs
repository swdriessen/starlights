using Starlights.Modules.Characters.Data.EntityFramework;
using Starlights.Platform.Components.Serilog;
using Starlights.Platform.Hosting;

namespace Modules.Characters.Data.EntityFramework.MigrationService;

/// <summary>Runs the Characters schema migration host.</summary>
public sealed class Program
{
    /// <summary>Builds and runs the migration host.</summary>
    public static void Main(string[] args)
    {
        var host = CreateHost(args);
        host.Run();
    }

    /// <summary>Builds the migration host without starting workers or applying migrations.</summary>
    public static IHost CreateHost(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.AddServiceDefaults();
        builder.Services.AddSingleton<ICurrentUser, MigrationCurrentUser>();

        builder.AddStarlightsPlatform(options =>
        {
            options.AdditionalAssemblies.Add(typeof(CharactersContext).Assembly);
            options.AddSerilogComponent();
        });

        builder.Services.AddHostedService<Worker>();

        return builder.Build();
    }
}