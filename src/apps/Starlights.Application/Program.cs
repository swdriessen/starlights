using Scalar.AspNetCore;
using Starlights.Modules.Identity;
using Starlights.Modules.FileManagement;
using Starlights.Modules.Identity.Accounts;
using Starlights.Modules.Characters.Data.EntityFramework;
using Starlights.Modules.Characters.Data.EntityFramework.EventProcessing;
using Starlights.Modules.Characters.Endpoints.Characters.CreateCharacter;
using Starlights.Modules.Characters.Services.Processing;
using Starlights.Modules.Elements;
using Starlights.Modules.Elements.Data.EntityFramework;
using Starlights.Modules.Elements.Data.EntityFramework.EventProcessing;
using Starlights.Modules.Elements.Endpoints.Installation;
using Starlights.Platform.Components.FastEndpoints;
using Starlights.Platform.Components.Serilog;
using Starlights.Platform.Eventing.EventPublisher;
using Starlights.Platform.Hosting;

namespace Starlights.Application;

public sealed class Program
{
    public static async Task<int> Main(string[] args)
    {
        var accountCommand = args is ["accounts", ..];
        var builder = WebApplication.CreateBuilder(accountCommand ? [] : args);
        builder.AddServiceDefaults();

        builder.AddIdentityModule();
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();
        builder.Services.AddValidation();
        builder.Services.AddProblemDetails();

        if (accountCommand)
        {
            await using var operatorHost = builder.Build();
            return await AccountCommands.RunAsync(operatorHost.Services, args[1..]);
        }

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod());
        });
        builder.AddFileManagementModule();

        // add the platform services and its modules
        builder.AddStarlightsPlatform(options =>
        {
            // characters module
            options.AdditionalAssemblies.Add(typeof(CharactersContext).Assembly);
            options.AdditionalAssemblies.Add(typeof(CreateCharacterEndpoint).Assembly);
            options.AdditionalAssemblies.Add(typeof(RegistrationProcessor).Assembly);
            options.AddEventProcessingComponent();

            // elements module
            options.AdditionalAssemblies.Add(typeof(ElementsModule).Assembly);
            options.AdditionalAssemblies.Add(typeof(ElementsContext).Assembly);
            options.AdditionalAssemblies.Add(typeof(InitializationEndpoint).Assembly);
            options.AddElementsEventProcessingComponent();

            // platform components
            options.AddFastEndpointsComponent();
            options.AddSerilogComponent();
            options.AdditionalAssemblies.Add(typeof(EventPublisherComponent).Assembly);
        });

        var app = builder.Build();

        app.UseExceptionHandler();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseIdentityApiProtection();
        app.MapIdentity();
        app.MapFileManagement();
        app.MapDefaultEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.Servers = [];

                options.WithTitle("Starlights API")
                    .HideClientButton()
                    .WithClassicLayout()
                    .WithTheme(ScalarTheme.Alternate)
                    .EnableDarkMode()
                    .HideModels()
                    .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch);
            });
        }

        // configure the platform and its modules
        app.UseStarlightsPlatform();
        app.UseCors();

        app.UseHttpsRedirection();
        await app.RunAsync();
        return 0;
    }
}
