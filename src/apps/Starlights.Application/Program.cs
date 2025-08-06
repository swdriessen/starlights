using Modules.Characters.Services.Processing;
using Scalar.AspNetCore;
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

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .SetIsOriginAllowed(_ =>
                    {
                        // Allow all origins for development purposes
                        return true;
                    })
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .AllowAnyMethod();
            });
        });

        // add the platform services and its modules
        builder.AddStarlightsPlatform(options =>
        {
            // characters module
            options.AdditionalAssemblies.Add(typeof(CharactersContext).Assembly);
            options.AdditionalAssemblies.Add(typeof(CreateCharacterEndpoint).Assembly);
            options.AdditionalAssemblies.Add(typeof(RegistrationManager).Assembly);

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
            app.MapScalarApiReference(options =>
            {
                options.Servers = [];
                options.WithTitle("Starlights API")
                    .WithClientButton(false)
                    .WithLayout(ScalarLayout.Modern)
                    .WithTheme(ScalarTheme.Alternate)
                    .WithDarkMode(true)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }

        // configure the platform and its modules
        app.UseStarlightsPlatform();
        app.UseCors();

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.Run();
    }
}
