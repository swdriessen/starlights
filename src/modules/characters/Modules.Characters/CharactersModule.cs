using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Characters.Services.Processing;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Platform.Eventing;
using Starlights.Platform.Hosting;

namespace Modules.Characters;

internal class CharactersModule : IPlatformServiceComponent
{
    public int RegistrationOrder => 1020;

    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IRegistrationManager, RegistrationManager>();

        builder.Services.AddTransient<IDomainEventHandler<RegistrationCreated>, RegistrationCreatedEventHandler>();
    }
}
