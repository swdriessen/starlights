using Starlights.Modules.Characters.Data.EntityFramework;
using Starlights.Modules.Characters.Endpoints.Entities.Characters.Create;
using Starlights.Modules.Elements;
using Starlights.Modules.Elements.Data.EntityFramework;
using Starlights.Modules.Elements.Endpoints.Installation;
using Starlights.Platform.Components.FastEndpoints;
using Starlights.Platform.Components.Serilog;
using Starlights.Platform.Hosting;

namespace Starlights.Application;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();

        // add the platform services and its modules
        builder.AddStarlightsPlatform(options =>
        {
            // characters module
            options.AdditionalAssemblies.Add(typeof(CharactersContext).Assembly);
            options.AdditionalAssemblies.Add(typeof(CreateCharacterEndpoint).Assembly);

            // elements module
            options.AdditionalAssemblies.Add(typeof(ElementsModule).Assembly);
            options.AdditionalAssemblies.Add(typeof(ElementsContext).Assembly);
            options.AdditionalAssemblies.Add(typeof(InitializationEndpoint).Assembly);

            // platform components
            options.AdditionalAssemblies.Add(typeof(FastEndpointsComponent).Assembly);
            options.AdditionalAssemblies.Add(typeof(SerilogComponent).Assembly);
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // configure the platform and its modules
        app.UseStarlightsPlatform();

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.Run();
    }
}
