using Microsoft.AspNetCore.Mvc;
using Starlights.Extensions.Platform.Data.EntityFramework;
using Starlights.Modules.Elements;
using Starlights.Modules.Elements.Data.EntityFramework;
using Starlights.Modules.Elements.Integration.Abstractions;
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
            options.AdditionalAssemblies.Add(typeof(ElementsModule).Assembly);

            // use entity framework for data persistence
            options.AdditionalAssemblies.Add(typeof(ExtensionsPlatformDataEntityFrameworkModule).Assembly);
            options.AdditionalAssemblies.Add(typeof(ElementsContext).Assembly);
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // configure the platform and its modules
        app.UseStarlightsPlatform(options =>
        {
            // configure
        });

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapGet("/hello", async (HttpContext _, [FromServices] IElementsModuleGateway e) =>
        {
            var elements = await e.GetElements("Ability");
            return Results.Ok("Hello, World!");
        })
        .WithName("HelloWorld");

        app.Run();
    }
}
