using Microsoft.AspNetCore.Mvc;
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
            // define assemblies in manifest or include them programmatically

            options.AdditionalAssemblies.Add(typeof(ElementsModule).Assembly);

            // use entity framework for data persistence
            //options.AdditionalAssemblies.Add(typeof(Persistence).Assembly);
            options.AdditionalAssemblies.Add(typeof(ElementsContext).Assembly);
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

        // used to test end-to-end until module offers its own API
        app.MapGet("/api/elements/{type}", async (string type, HttpContext _, [FromServices] IElementsModuleGateway gateway) =>
        {
            var elements = await gateway.GetElements(type);
            return Results.Ok(elements);
        })
        .WithName("Elements");

        app.Run();
    }
}
