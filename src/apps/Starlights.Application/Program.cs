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
        builder.AddStarlightsPlatform();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // configure the platform and its modules
        app.UseStarlightsPlatform();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapGet("/hello", (HttpContext _) => Results.Ok("Hello, World!"))
        .WithName("HelloWorld");

        app.Run();
    }
}
