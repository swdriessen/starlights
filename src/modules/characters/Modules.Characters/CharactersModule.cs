using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Modules.Characters.Domain.Services;
using Starlights.Modules.Characters.Services;
using Starlights.Modules.Characters.Services.Processing;
using Starlights.Modules.Characters.Services.Processing.Behaviors;
using Starlights.Platform.Eventing.EventPublisher;
using Starlights.Platform.Hosting;

namespace Starlights.Modules.Characters;

internal class CharactersModule : IPlatformServiceComponent
{
    public int RegistrationOrder => 1020;

    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IRegistrationProcessor, RegistrationProcessor>();
        builder.Services.AddScoped<IRegistrationManager, RegistrationManager>();

        builder.Services.AddScoped<ClassManagementService>();
        builder.Services.AddScoped<ICharacterCreationService, CharacterCreationService>();
        builder.Services.AddScoped<ICharacterAbilitiesUpdateService, CharacterAbilitiesUpdateService>();
        builder.Services.AddScoped<StatisticsCalculator>();

        // registration behaviors
        builder.Services.AddScoped<IRegistrationBehavior, SkillRegistrationBehavior>();
        builder.Services.AddScoped<IRegistrationBehavior, SavingThrowRegistrationBehavior>();
        builder.Services.AddScoped<IRegistrationBehavior, AbilityRegistrationBehavior>();
        builder.Services.AddScoped<IRegistrationBehavior, ClassRegistrationBehavior>();

        // event handlers
        builder.Services.AddDomainEventHandlersFrom(typeof(CharactersModule).Assembly);
    }
}
