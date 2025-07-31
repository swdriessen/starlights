using Starlights.Modules.Elements;
using Starlights.Modules.Elements.Data.EntityFramework;
using Starlights.Modules.Elements.Endpoints.Installation;
using Starlights.Platform.Components.FastEndpoints;
using Starlights.Platform.Hosting;

namespace Starlights.Application;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();

        // add the platform services and its modules
        builder.AddStarlightsPlatform(options =>
        {
            // define assemblies in manifest or include them programmatically
            options.AdditionalAssemblies.Add(typeof(ElementsModule).Assembly);

            // use entity framework for data persistence
            options.AdditionalAssemblies.Add(typeof(ElementsContext).Assembly);
            options.AdditionalAssemblies.Add(typeof(InitializationEndpoint).Assembly);
            options.AdditionalAssemblies.Add(typeof(FastEndpointsComponent).Assembly);
        });

        var app = builder.Build();

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
