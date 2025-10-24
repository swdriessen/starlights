using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Modules.Characters.Domain.Services;
using Starlights.Modules.Characters.Services.Processing;
using Starlights.Modules.Characters.Services.Processing.Behaviors;
using Starlights.Modules.Characters.Services.Statistics;
using Starlights.Modules.Characters.Services.Statistics.Initializers;
using Starlights.Modules.Characters.Services.Statistics.Processors;
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

        // statistics seed processors (run before direct value rules)
        builder.Services.AddScoped<IStatisticsCalculationInitializer, CharacterStatisticsInitializer>();
        builder.Services.AddScoped<IStatisticsCalculationInitializer, ClassStatisticsInitializer>();
        builder.Services.AddScoped<IStatisticsCalculationInitializer, AbilitiesStatisticsInitializer>();
        builder.Services.AddScoped<IStatisticsCalculationInitializer, SavingThrowStatisticsInitializer>();
        builder.Services.AddScoped<IStatisticsCalculationInitializer, SkillStatisticsInitializer>();
        builder.Services.AddScoped<IStatisticGroupProcessor, AbilitiesGroupProcessor>();
        builder.Services.AddScoped<IStatisticGroupProcessor, ProficiencyGroupProcessor>();
        builder.Services.AddScoped<IStatisticsPostProcessor, SkillStatisticsPostProcessor>();
        builder.Services.AddScoped<IStatisticsPostProcessor, SavingThrowStatisticsPostProcessor>();


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
