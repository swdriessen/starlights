using Microsoft.Extensions.DependencyInjection;
using Starlights.Modules.Elements.Application.Services;
using Starlights.Modules.Elements.Integration.Abstractions;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Modules.Elements;

public sealed class ElementsModule : IPlatformModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IElementsModuleGateway, ElementsModuleGateway>();
    }
}
