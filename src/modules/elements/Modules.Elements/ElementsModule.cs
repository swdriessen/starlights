using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Modules.Elements.Integration;
using Starlights.Modules.Elements.Services;
using Starlights.Platform.Eventing.EventPublisher;
using Starlights.Platform.Hosting;

namespace Starlights.Modules.Elements;

public sealed class ElementsModule : IPlatformModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IElementsModuleQueries, ElementsModuleQueries>();
        builder.Services.AddScoped<IElementsModuleInitializer, ElementsModuleInitializer>();

        builder.Services.AddDomainEventHandlersFrom(typeof(ElementsModule).Assembly);
    }
}
